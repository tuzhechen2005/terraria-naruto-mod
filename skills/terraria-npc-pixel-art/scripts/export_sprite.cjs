#!/usr/bin/env node
// Export an existing generated sprite. Does not draw or repair facial features.
const fs = require('node:fs');
const path = require('node:path');
const { parseArgs } = require('node:util');
const options = {
 input:{type:'string'}, out:{type:'string'}, prefix:{type:'string',default:'NPC'},
 canvas:{type:'string',default:'64x80'}, height:{type:'string',default:'62'},
 baseline:{type:'string',default:'76'}, pixel:{type:'string',default:'1'},
 'phase-x':{type:'string',default:'0.5'}, 'phase-y':{type:'string',default:'0.5'},
 alpha:{type:'string',default:'128'}, bbox:{type:'string'}, scale:{type:'string'},
 facing:{type:'string',default:'right'}, overwrite:{type:'boolean',default:false},
 help:{type:'boolean',default:false}
};
function main(){
 const {values:a}=parseArgs({options});
 if(a.help){console.log('Export an existing PNG sprite to exact game-size frames.\n--input PNG --out DIR [--prefix NPC] [--canvas 64x80]\n[--height 62 | --scale RATIO] [--baseline 76] [--pixel 1]\n[--phase-x 0.5] [--phase-y 0.5] [--alpha 128]\n[--bbox x0,y0,x1,y1] [--facing right|left] [--overwrite]\nRequires pngjs (normal Node resolution or NODE_PATH). Baseline is exclusive.');return;}
 if(!a.input||!a.out)throw Error('--input and --out are required');
 let PNG;try{({PNG}=require('pngjs'));}catch{throw Error('pngjs unavailable; locate the workspace dependency package directory and set NODE_PATH');}
 const integer=(s,label,min=1,max=Infinity)=>{const n=Number(s);if(!Number.isInteger(n)||n<min||n>max)throw Error(label+' must be an integer in range');return n;};
 const positive=(s,label)=>{const n=Number(s);if(!Number.isFinite(n)||n<=0)throw Error(label+' must be positive');return n;};
 const phase=(s,label)=>{const n=Number(s);if(!Number.isFinite(n)||n<0||n>=1)throw Error(label+' must be >=0 and <1');return n;};
 if(!/^[a-zA-Z0-9_-]+$/.test(a.prefix))throw Error('prefix must contain only letters, digits, _ or -');
 if(!['left','right'].includes(a.facing))throw Error('facing must be right or left');
 const parts=a.canvas.split('x');if(parts.length!==2)throw Error('canvas must be WxH');
 const fw=integer(parts[0],'canvas width',1,4096),fh=integer(parts[1],'canvas height',1,4096);
 const pixel=integer(a.pixel,'pixel',1,8),baseline=integer(a.baseline,'baseline',1,fh);
 const threshold=integer(a.alpha,'alpha',1,255),px=phase(a['phase-x'],'phase-x'),py=phase(a['phase-y'],'phase-y');
 if(fw%pixel||fh%pixel||baseline%pixel)throw Error('canvas and baseline must align to pixel size');
 const input=path.resolve(a.input),out=path.resolve(a.out);
 const src=PNG.sync.read(fs.readFileSync(input));
 let box;
 if(a.bbox){box=a.bbox.split(',').map(Number);if(box.length!==4||box.some(n=>!Number.isInteger(n)))throw Error('bbox must be four integers');}
 else{let x0=src.width,y0=src.height,x1=0,y1=0;for(let y=0;y<src.height;y++)for(let x=0;x<src.width;x++)if(src.data[(y*src.width+x)*4+3]>=threshold){x0=Math.min(x0,x);y0=Math.min(y0,y);x1=Math.max(x1,x+1);y1=Math.max(y1,y+1);}box=[x0,y0,x1,y1];}
 const [x0,y0,x1,y1]=box;if(x0<0||y0<0||x1>src.width||y1>src.height||x1<=x0||y1<=y0)throw Error('empty or invalid source bbox');
 const requestedHeight=integer(a.height,'height');
 if(!a.scale&&requestedHeight%pixel)throw Error('height must align to pixel size');
 const scale=a.scale?positive(a.scale,'scale'):requestedHeight/(y1-y0);
 const rows=Math.max(1,Math.round((y1-y0)*scale/pixel)),cols=Math.max(1,Math.round((x1-x0)*scale/pixel));
 const top=baseline-rows*pixel,left=Math.floor((fw-cols*pixel)/(2*pixel))*pixel;
 if(top<0||left<0||left+cols*pixel>fw)throw Error('sprite would be clipped; enlarge canvas or adjust scale/baseline');
 const grid=new PNG({width:cols,height:rows});
 for(let y=0;y<rows;y++)for(let x=0;x<cols;x++){
  const sx=x0+Math.min(x1-x0-1,Math.floor((x+px)*(x1-x0)/cols)),sy=y0+Math.min(y1-y0-1,Math.floor((y+py)*(y1-y0)/rows));
  const si=(sy*src.width+sx)*4,di=(y*cols+x)*4;
  if(src.data[si+3]>=threshold){src.data.copy(grid.data,di,si,si+3);grid.data[di+3]=255;}
 }
 const frame=new PNG({width:fw,height:fh});
 for(let y=0;y<rows;y++)for(let x=0;x<cols;x++)for(let dy=0;dy<pixel;dy++)for(let dx=0;dx<pixel;dx++){
  grid.data.copy(frame.data,((top+y*pixel+dy)*fw+left+x*pixel+dx)*4,(y*cols+x)*4,(y*cols+x+1)*4);
 }
 const mirror=new PNG({width:fw,height:fh}),zoom=new PNG({width:fw*6,height:fh*6});
 let visibleTop=fh,visibleBottom=-1,visibleLeft=fw,visibleRight=-1;
 for(let y=0;y<fh;y++)for(let x=0;x<fw;x++){
  const i=(y*fw+x)*4;frame.data.copy(mirror.data,(y*fw+fw-x-1)*4,i,i+4);
  if(frame.data[i+3]){visibleTop=Math.min(visibleTop,y);visibleBottom=Math.max(visibleBottom,y);visibleLeft=Math.min(visibleLeft,x);visibleRight=Math.max(visibleRight,x);}
 }
 if(visibleBottom<0)throw Error('sampling produced an empty sprite; adjust bbox or sampling phase');
 for(let y=0;y<zoom.height;y++)for(let x=0;x<zoom.width;x++){const si=(Math.floor(y/6)*fw+Math.floor(x/6))*4;frame.data.copy(zoom.data,(y*zoom.width+x)*4,si,si+4);}
 const right=a.facing==='right'?frame:mirror,leftFrame=a.facing==='left'?frame:mirror;
 const manifest={input,sourceBox:box,canvas:[fw,fh],grid:[cols,rows],pixel,scale,samplePhase:[px,py],alphaThreshold:threshold,alphaValues:[0,255],sourceFacing:a.facing,baselineExclusive:baseline,actualBaselineExclusive:visibleBottom+1,actualBodyHeight:visibleBottom-visibleTop+1,visibleBox:[visibleLeft,visibleTop,visibleRight+1,visibleBottom+1],displayScale:1};
 if(visibleBottom+1!==baseline)manifest.warning='Sampled feet did not reach the requested baseline; inspect phase/bbox before integration.';
 const files=[['Right.png',PNG.sync.write(right)],['Left.png',PNG.sync.write(leftFrame)],['6x.png',PNG.sync.write(zoom)],['manifest.json',JSON.stringify(manifest,null,2)+'\n']].map(([suffix,data])=>[path.join(out,a.prefix+'_'+suffix),data]);
 for(const [target] of files){if(path.resolve(target)===input)throw Error('output must not replace the input');if(fs.existsSync(target)&&!a.overwrite)throw Error('output exists: '+target+'; use another prefix/directory or --overwrite for an intended update');}
 fs.mkdirSync(out,{recursive:true});for(const [target,data]of files)fs.writeFileSync(target,data);
 console.log(JSON.stringify(manifest,null,2));
}
try{main();}catch(e){console.error(e.message);process.exitCode=1;}
