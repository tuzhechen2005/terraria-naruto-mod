// Deterministic export only. No drawing or replacement of face pixels.
const fs = require('node:fs');
const path = require('node:path');
const {PNG} = require(process.env.NPC_NODE_MODULES + '/pngjs');
const out = __dirname;
const src = PNG.sync.read(fs.readFileSync(path.join(out,'source/generated_Iruka_Idle_B.png')));
const mouthEdit = PNG.sync.read(fs.readFileSync(path.join(out,'source/generated_no_mouth_edit.png')));
if(mouthEdit.width!==src.width||mouthEdit.height!==src.height)throw Error('Mouth edit must match source canvas');
const noMouth = PNG.sync.read(fs.readFileSync(path.join(out,'source/generated_Iruka_Idle_B.png')));
// Composite only the local image-tool edit. Keep all other original pixels unchanged.
const mouthBox=[656,620,720,652];
for(let y=mouthBox[1];y<mouthBox[3];y++)for(let x=mouthBox[0];x<mouthBox[2];x++){
 const i=(y*src.width+x)*4;mouthEdit.data.copy(noMouth.data,i,i,i+4);
}
fs.writeFileSync(path.join(out,'source/Iruka_B_no_mouth.png'),PNG.sync.write(noMouth));
const eyeEdit=PNG.sync.read(fs.readFileSync(path.join(out,'source/generated_eye_edit.png')));
if(eyeEdit.width!==src.width||eyeEdit.height!==src.height)throw Error('Eye edit must match source canvas');
const largeEye=PNG.sync.read(PNG.sync.write(noMouth));
const eyeFaceBox=[612,470,708,636];
for(let y=eyeFaceBox[1];y<eyeFaceBox[3];y++)for(let x=eyeFaceBox[0];x<eyeFaceBox[2];x++){
 const i=(y*src.width+x)*4;eyeEdit.data.copy(largeEye.data,i,i,i+4);
}
// Fit the tool-created eye to four native screen rows, preserving its width.
// This changes only vertical scale of that generated local eye patch.
for(let y=477;y<566;y++)for(let x=621;x<701;x++){
 const sy=490+Math.min(68,Math.floor((y-477+.5)*69/89));
 const si=(sy*src.width+x)*4,di=(y*src.width+x)*4;
 eyeEdit.data.copy(largeEye.data,di,si,si+4);
}
fs.writeFileSync(path.join(out,'source/Iruka_B_eye_two_cells.png'),PNG.sync.write(largeEye));
let x0=src.width,y0=src.height,x1=0,y1=0;
for(let y=0;y<src.height;y++)for(let x=0;x<src.width;x++){
 if(src.data[(y*src.width+x)*4+3]>=128){x0=Math.min(x0,x);y0=Math.min(y0,y);x1=Math.max(x1,x+1);y1=Math.max(y1,y+1);}
}
const manifest={source:'source/generated_Iruka_Idle_B.png',finalSource:'source/Iruka_B_eye_two_cells.png',mouthEditBox:mouthBox,eyeFaceEditBox:eyeFaceBox,eyeHeightScreenPixels:4,sourceBox:[x0,y0,x1,y1],samplePhase:[0.8,0.5],exports:[]};
function save(png,name){fs.writeFileSync(path.join(out,name),PNG.sync.write(png));}
for(const [rows,fw,fh,baseline,label,pixel] of [[31,64,80,76,'native',2],[23,56,56,54,'legacy',2],[62,64,80,76,'final',1]]){
 const sampledSrc=label==='final'?largeEye:src;
 const cols=Math.round((x1-x0)*rows/(y1-y0));
 const grid=new PNG({width:cols,height:rows});
 for(let y=0;y<rows;y++)for(let x=0;x<cols;x++){
  const sx=x0+Math.min(x1-x0-1,Math.floor((x+.8)*(x1-x0)/cols));
  const sy=y0+Math.min(y1-y0-1,Math.floor((y+.5)*(y1-y0)/rows));
  const si=(sy*src.width+sx)*4,di=(y*cols+x)*4;
  if(sampledSrc.data[si+3]>=128){sampledSrc.data.copy(grid.data,di,si,si+3);grid.data[di+3]=255;}
 }
 save(grid,'Iruka_Grid_'+label+'.png');
 const frame=new PNG({width:fw,height:fh});
 const left=Math.floor((fw-cols*pixel)/(2*pixel))*pixel,top=baseline-rows*pixel;
 for(let y=0;y<rows;y++)for(let x=0;x<cols;x++)for(let dy=0;dy<pixel;dy++)for(let dx=0;dx<pixel;dx++){
  grid.data.copy(frame.data,((top+y*pixel+dy)*fw+left+x*pixel+dx)*4,(y*cols+x)*4,(y*cols+x+1)*4);
 }
 save(frame,'Iruka_Idle_'+label+'_Right.png');
 const flipped=new PNG({width:fw,height:fh});
 for(let y=0;y<fh;y++)for(let x=0;x<fw;x++){
  frame.data.copy(flipped.data,(y*fw+fw-x-1)*4,(y*fw+x)*4,(y*fw+x+1)*4);
 }
 save(flipped,'Iruka_Idle_'+label+'_Left.png');
 // Enlarged inspection of the exact frame, not a second resampling of the source.
 const zoom=new PNG({width:fw*6,height:fh*6});
 for(let y=0;y<zoom.height;y++)for(let x=0;x<zoom.width;x++){
  const si=(Math.floor(y/6)*fw+Math.floor(x/6))*4;
  frame.data.copy(zoom.data,(y*zoom.width+x)*4,si,si+4);
 }
 save(zoom,'Iruka_Idle_'+label+'_6x.png');
 let visibleRows=[];for(let y=0;y<rows;y++)if([...Array(cols)].some((_,x)=>grid.data[(y*cols+x)*4+3]))visibleRows.push(y);
 const body=pixel*(visibleRows.at(-1)-visibleRows[0]+1);
 manifest.exports.push({label,canvas:[fw,fh],grid:[cols,rows],pixel,bodyHeight:body,baselineExclusive:baseline,displayScale:label==='legacy'?62.5/46:1,alpha:[0,255]});
}
fs.writeFileSync(path.join(out,'FRAME_MANIFEST.json'),JSON.stringify(manifest,null,2)+'\n');
console.log(JSON.stringify(manifest));
