'use strict';
const input=document.querySelector('#search'), results=document.querySelector('#search-results');
const normalized=s=>s.toLowerCase().normalize('NFKC');
const aliases={'再不斩':'zabuza','白眼':'byakugan','苦无':'kunai','写轮眼':'sharingan','八门':'gates','boss':'boss','查克拉':'chakra','替身':'substitution'};
function search(){
 const q=normalized(input.value.trim());results.replaceChildren();
 if(!q){results.hidden=true;return;}
 const hits=window.WIKI_INDEX.map(e=>({e,rank:normalized(e.title).includes(q)?3:normalized(e.summary).includes(q)?2:normalized(e.keywords+' '+e.id+' '+e.category).includes(q)||e.id===aliases[q]?1:0})).filter(x=>x.rank).sort((a,b)=>b.rank-a.rank).slice(0,12);
 results.hidden=false;
 if(!hits.length){const p=document.createElement('p');p.textContent='没有找到词条，试试「卷轴」「首领」或「查克拉」。';results.append(p);return;}
 for(const {e} of hits){const a=document.createElement('a');a.href=e.id+'.html';const name=document.createElement('strong');name.textContent=e.title;const meta=document.createElement('small');meta.textContent=e.category+' · '+e.summary;a.append(name,meta);results.append(a);}
}
input.addEventListener('input',search);
input.addEventListener('focus',()=>{if(input.value)search();});
document.querySelector('#search-form').addEventListener('submit',e=>{e.preventDefault();const first=results.querySelector('a');if(first)location.href=first.href;});
document.addEventListener('keydown',e=>{if(e.key==='/'&&!/INPUT|TEXTAREA/.test(document.activeElement.tagName)){e.preventDefault();input.focus();}if(e.key==='Escape'){results.hidden=true;input.blur();}if(e.key==='ArrowDown'&&document.activeElement===input){const first=results.querySelector('a');if(first){e.preventDefault();first.focus();}}});
document.addEventListener('click',e=>{if(!e.target.closest('#search-form'))results.hidden=true;});
const menu=document.querySelector('#menu');menu.addEventListener('click',()=>{const open=document.body.classList.toggle('nav-open');menu.setAttribute('aria-expanded',String(open));});
const filter=document.querySelector('#category-filter');
if(filter)filter.addEventListener('input',()=>{let count=0;for(const card of document.querySelectorAll('.category-list .entry-card')){card.hidden=!normalized(card.textContent).includes(normalized(filter.value));if(!card.hidden)count++;}for(const group of document.querySelectorAll('.item-group'))group.hidden=![...group.querySelectorAll('.entry-card')].some(row=>!row.hidden);document.querySelector('#empty-filter').hidden=count!==0;});
