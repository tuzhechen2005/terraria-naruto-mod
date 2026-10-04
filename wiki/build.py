#!/usr/bin/env python3
"""生成可直接托管的静态百科。运行：python3 wiki/build.py（需要 Pillow）。"""
import hashlib, html, json, re, shutil, subprocess
from pathlib import Path
from PIL import Image
from content import ENTRIES, CATEGORIES
ROOT=Path(__file__).resolve().parents[1]
MOD=ROOT/'ShinobiPrototype'
OUT=ROOT/'wiki/site'
ASSETS=OUT/'assets'
ASSETS.mkdir(parents=True,exist_ok=True)
E=lambda x:html.escape(str(x),quote=True)
SHA=subprocess.check_output(['git','rev-parse','HEAD'],cwd=ROOT,text=True).strip()
VERSION=re.search(r'^version\s*=\s*(.+)$',(MOD/'build.txt').read_text(),re.M)[1]
manifest={}
def source_path(p): return (MOD/p).resolve()
def constant(path,key):
    match=re.search(r'const int '+re.escape(key)+r'\s*=\s*(\d+)',source_path(path).read_text())
    if not match: raise ValueError(f'Missing literal constant {path}: {key}')
    return match[1]
def asset(p):
    if not p: return None
    src=source_path(p)
    if not src.exists(): raise FileNotFoundError(src)
    manifest[str(src.relative_to(ROOT))]=hashlib.sha256(src.read_bytes()).hexdigest()
    name=src.stem+'.png'
    with Image.open(src) as im:
        im=im.convert('RGBA')
        if p.startswith('Content/NPCs/') and not re.search(r'_\d+\.png$',p):
            # NPC 纵向帧表：只提取第一帧，保留游戏本身的配色。
            heights={'Kakashi':56,'Tazuna':56,'Ibiki':64,'Hiruzen':56,'Anko':64,'Hayate':64,'HakuForest':56,'DemonBrotherGozu':56,'ForestCanopyCandidate':88,'RainGenin':88}
            h=heights.get(src.stem)
            if h: im=im.crop((0,0,im.width,min(h,im.height)))
        bounds=im.getchannel('A').getbbox()
        if bounds: im=im.crop(bounds)
        im.save(ASSETS/name)
    return 'assets/'+name+'?v='+hashlib.sha256((ASSETS/name).read_bytes()).hexdigest()[:10]
lookup={e['id']:e for e in ENTRIES}
if len(lookup)!=len(ENTRIES): raise ValueError('duplicate id')
for e in ENTRIES:
    if e['id'] in ['dosu','gaara','neji','orochimaru']:
        name={'dosu':'Dosu','gaara':'Gaara','neji':'Neji','orochimaru':'Orochimaru'}[e['id']]
        e['stats']['基础生命']=constant('Common/ExamBossRules.cs',name+'Life')
        e['stats']['基础防御']=constant('Common/ExamBossRules.cs',name+'Defense')
    if e['id']=='zabuza': e['stats']['基础生命']=constant('Common/ZabuzaCombatRules.cs','BossMaxLife')
    if e['id']=='haku': e['stats']['基础生命']=constant('Common/WaveDuoRules.cs','HakuMaxLife')
    for id in e['related']:
        if id not in lookup: raise ValueError(id)
    if e['category']=='item':
        e['source'] += ['Common/WaveLootRules.cs','Common/Systems/WaveRewards.cs','Common/Players/WaveRewardPlayer.cs'] if e['id'] in ['kubikiribocho','senbon','water-dragon','ice-mirror','medal','bag','zabuza-head','haku-mask','zabuza-trophy','haku-trophy'] else []
        e['source'] += ['Common/StyleCoreRules.cs','Common/Players/StyleCorePlayer.cs','Common/Players/ChuninExamPlayer.cs','Content/NPCs/Gaara.cs','Content/NPCs/Neji.cs','Content/NPCs/Orochimaru.cs'] if e['id'] in ['sharingan','gates','byakugan','leg-weights','headband'] else []
        e['source'] += ['Common/ChakraRules.cs','Common/Players/ChakraPlayer.cs','Common/Systems/ChakraCrystalWorld.cs'] if e['id'] in ['pill','crystal'] else []
        e['source'] += ['Common/Players/StoryPlayer.cs','Content/NPCs/Kakashi.cs','Content/NPCs/TazunaBridge.cs','Common/Systems/DeathForestSystem.cs','Common/Players/ChuninExamPlayer.cs','Content/NPCs/ExamProctors.cs'] if e['id'] in ['kunai','handbook','insignia','recommendation','heaven','earth'] else []
    for p in e['source']:
        src=source_path(p)
        if not src.exists(): raise FileNotFoundError(src)
        manifest[str(src.relative_to(ROOT))]=hashlib.sha256(src.read_bytes()).hexdigest()
    if e['category']=='item':
        text=source_path(e['source'][0]).read_text()
        if not e['stats']:
            for key,label in [('damage','基础伤害'),('useTime','使用时间'),('knockBack','击退'),('mana','魔力消耗')]:
                m=re.search(r'Item\.'+key+r'\s*=\s*(\d+(?:\.\d+)?)[fF]?;',text)
                if m: e['stats'][label]=m[1]+(' 帧' if key=='useTime' else '')
            m=re.search(r'Item.DamageType\s*=\s*DamageClass\.(\w+)',text)
            if m: e['stats']['伤害类型']={'Melee':'近战','Ranged':'远程','Magic':'魔法','Summon':'召唤'}.get(m[1],m[1])
        if not e['image'] and 'override string Texture' not in text:
            candidate=e['source'][0].replace('.cs','.png')
            if source_path(candidate).exists(): e['image']=candidate
    e['asset']=asset(e['image'])
shutil.copyfile(ROOT/'docs/images/mod-icon.png',ASSETS/'terruto.png')
for background in ['KonohaFar','KonohaMid']:
    src=MOD/'Backgrounds'/f'{background}.png'
    shutil.copyfile(src,ASSETS/f'{background}.png')
    manifest[str(src.relative_to(ROOT))]=hashlib.sha256(src.read_bytes()).hexdigest()
for name in ['style.css','app.js']: shutil.copyfile(ROOT/'wiki'/name,ASSETS/name)
CSS_VERSION=hashlib.sha256((ASSETS/'style.css').read_bytes()).hexdigest()[:10]
JS_VERSION=hashlib.sha256((ASSETS/'app.js').read_bytes()).hexdigest()[:10]
index=[dict(id=e['id'],title=e['title'],category=CATEGORIES[e['category']],summary=e['summary'],keywords=' '.join(str(x) for x in e['sections'].values())) for e in ENTRIES]
(ASSETS/'search-index.js').write_text('window.WIKI_INDEX='+json.dumps(index,ensure_ascii=False)+';',encoding='utf-8')
SEARCH_VERSION=hashlib.sha256((ASSETS/'search-index.js').read_bytes()).hexdigest()[:10]
def link(id,label=None,cls=''):
    return f'<a class="{cls}" href="{id}.html">{E(label or lookup[id]["title"])}</a>'
def picture(e,cls=''):
    return f'<img class="sprite {cls}" src="{e["asset"]}" alt="{E(e["title"])}" loading="lazy">' if e['asset'] else '<span class="text-icon" aria-hidden="true">'+{'guide':'卷','mechanic':'术','place':'域','item':'物'}.get(e['category'],'忍')+'</span>'
def card(e):
    return f'<a class="entry-card" href="{e["id"]}.html">{picture(e)}<div><span class="eyebrow">{E(e["tag"] or CATEGORIES[e["category"]])}</span><strong>{E(e["title"])}</strong><p>{E(e["summary"])}</p></div><span class="arrow" aria-hidden="true">↗</span></a>'
def shell(title,body,current='index',desc='Terruto 火影模组中文百科：道具、首领、人物、地区、机制与推进路线。'):
    def navlink(c,n):
        return f'<a href="{c}.html" '+('aria-current="page"' if current==c else '')+f'>{E(n)}</a>'
    nav=navlink('index','百科首页')+'<div class="nav-caption">游戏内容</div>'+''.join(navlink(c,CATEGORIES[c]) for c in ['item','boss','npc','enemy','place'])+'<div class="nav-caption">攻略与机制</div>'+''.join(navlink(c,CATEGORIES[c]) for c in ['guide','mechanic'])+navlink('progression','Boss 推进路线')
    return f'''<!doctype html><html lang="zh-CN"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>{E(title)} · Terruto Wiki</title><meta name="description" content="{E(desc)}"><link rel="icon" href="assets/terruto.png"><link rel="stylesheet" href="assets/style.css?v={CSS_VERSION}"><script defer src="assets/search-index.js?v={SEARCH_VERSION}"></script><script defer src="assets/app.js?v={JS_VERSION}"></script></head><body><a class="skip" href="#main">跳到正文</a><div class="network-bar"><a href="index.html"><b>Terruto Wiki</b></a><span>Terraria × Naruto</span><div><a href="about.html">关于百科</a><a href="https://github.com/tuzhechen2005/terraria-naruto-mod">GitHub ↗</a></div></div><div class="masthead"><a href="index.html" aria-label="Terruto 百科首页"><img src="assets/terruto.png" alt=""><div><b>TERRUTO</b><span>火影模组中文百科</span></div></a></div><div class="wiki-frame"><aside class="sidebar"><div class="sidebar-title">导航</div><nav aria-label="主导航">{nav}</nav><div class="sidebar-bottom"><b>当前版本 {E(VERSION)}</b><small>波之国 · 中忍考试</small><a href="getting-started.html">新手入门</a><a href="roadmap.html">后续开发路线</a><a href="about.html">内容依据与维护</a></div></aside><div class="workspace"><header><button id="menu" aria-label="展开导航" aria-expanded="false">☰</button><a class="page-tab" href="index.html">百科首页</a><a class="page-tab active" href="#main">{E(title)}</a><form role="search" id="search-form"><label class="sr-only" for="search">搜索百科</label><input id="search" type="search" autocomplete="off" placeholder="搜索 Terruto Wiki"><span aria-hidden="true">⌕</span><div id="search-results" hidden aria-live="polite"></div></form></header><main id="main">{body}</main><footer><span>Terruto Wiki · 非官方火影同人模组百科</span><span>当前版本 {E(VERSION)} · <a href="about.html">内容依据与更新说明</a></span></footer></div></div></body></html>'''
def write(name,title,body,current='index',desc=None):
    (OUT/(name+'.html')).write_text(shell(title,body,current,desc or 'Terruto 火影模组中文百科：'+title),encoding='utf-8')
def crumb(text): return '<div class="breadcrumb"><a href="index.html">百科首页</a><span>/</span>'+E(text)+'</div>'
def icon_link(id):
    e=lookup[id]
    return f'<a class="icon-link" href="{id}.html">{picture(e)}<span>{E(e["title"])}</span></a>'
def panel(title,ids,url=None,cls=''):
    return f'<section class="wiki-panel {cls}"><h2>'+ (f'<a href="{url}.html">{title}</a>' if url else title)+'</h2><div class="icon-links">'+''.join(icon_link(id) for id in ids)+'</div></section>'
body=f'<section class="wiki-welcome"><h1>欢迎来到 <strong>Terruto 中文百科</strong></h1><p>收录火影模组中的道具、角色与冒险流程。</p><div class="wiki-counts">{len(ENTRIES)} 个词条 · 7 个分类 · 当前开发版 {E(VERSION)}</div><div class="welcome-links">'+link('getting-started','新手入门')+' · '+link('progression','Boss 推进路线')+' · '+link('about','内容与版本说明')+'</div></section>'
body+='<div class="home-intro"><section class="wiki-panel"><h2>关于模组</h2><div class="panel-body"><p><b>Terruto</b> 将《火影忍者》第一部的故事融入泰拉瑞亚的冒险。从木叶出发，在卡卡西的引导下前往波之国，再回村参加中忍考试。</p><p>保留原版全部 Boss 与四种职业，加入独立查克拉、木头替身、潜伏、结印忍术和流派核心。目前内容推进到中忍考试。</p><div class="inline-links">'+link('wave-guide','波之国攻略')+' · '+link('exam-guide','中忍考试攻略')+' · '+link('styles','流派系统')+'</div></div></section><section class="wiki-panel version-panel"><h2>当前版本</h2><dl><dt>Terruto</dt><dd>'+E(VERSION)+'</dd><dt>游戏环境</dt><dd>tModLoader 1.4.4</dd><dt>可玩篇章</dt><dd>波之国 / 中忍考试</dd><dt>首次游玩</dt><dd>'+link('getting-started','需要新建世界')+'</dd></dl></section></div>'
body+='<div class="home-panels">'
body+=panel('道具与忍具',['kubikiribocho','senbon','water-dragon','ice-mirror','wraps','leg-weights','sharingan','gates','byakugan','clone-scroll','fireball-scroll','chidori-scroll'],'item','wide')
body+=panel('首领与遭遇战',['zabuza','haku','orochimaru','dosu','gaara','neji'],'boss','boss-panel')
body+=panel('剧情人物',['kakashi','tazuna','ibiki','anko','hayate','hiruzen','haku-forest'],'npc')
body+=panel('机制与修行',['chakra','substitution','stealth','hand-seals','styles','taijutsu','vow','status'],'mechanic')
body+=panel('地区与建筑',['konoha','bridge','forest','tower','stadium'],'place')
body+=panel('敌怪图鉴',['demon-brothers','candidates','rain-genin'],'enemy')
body+=panel('冒险指南',['getting-started','wave-guide','lake','exam-guide','written','progression'],'guide')
body+='</div><div class="notice"><b>开发版说明</b>考试篇仍在调整。后续木叶崩溃、仙术与尾兽属于规划内容，见 '+link('roadmap','开发路线')+'。</div>'
write('index','首页',body)
ITEM_GROUPS=[('weapons','武器与忍术',['kunai','kubikiribocho','senbon','water-dragon','ice-mirror','wraps','leg-weights']),('seal-jutsu','结印忍术卷轴',['clone-scroll','fireball-scroll','chidori-scroll']),('cores','流派核心',['sharingan','gates','byakugan']),('accessories','饰品',['medal','headband']),('consumables','消耗品与材料',['pill','crystal','insignia','bag']),('summons','首领召唤物',['wave-scroll','sound-token','sand-gourd','snake-skin','neji-scroll']),('quest','任务与工具',['handbook','recommendation','heaven','earth','blueprint']),('vanity','时装与装饰',['zabuza-head','haku-mask','zabuza-trophy','haku-trophy'])]
for c,n in CATEGORIES.items():
    entries=[e for e in ENTRIES if e['category']==c]
    body=crumb(n)+f'<div class="page-heading"><h1>{n}</h1><p>本页列出当前版本的 {len(entries)} 个词条。点击名称查看详细资料。</p></div>'
    if c=='item':
        body+='<div class="notice">武器按近战、远程、魔法、召唤与体术区分；流派核心与职业可以自由搭配。</div><nav class="toc" aria-label="本页目录"><b>目录</b>'+''.join(f'<a href="#{id}">{name}</a>' for id,name,ids in ITEM_GROUPS)+'</nav>'
    body+='<label class="filter-label">筛选名称 <input id="category-filter" type="search" placeholder="名称或关键词…"></label>'
    if c=='item':
        body+='<div class="category-list">'
        for id,name,ids in ITEM_GROUPS:
            body+=f'<section class="item-group" id="{id}"><h2>{name}</h2>'+ ('<p class="notice">分身术开局自带；豪火球与千鸟已实现，可用 /m0 seals 开发试用，正常获取来源尚未接入。</p>' if id=='seal-jutsu' else '')+f'<div class="table-wrap"><table class="wiki-table"><thead><tr><th>物品</th>'+('<th>伤害类型</th><th>伤害</th><th>使用时间</th>' if id=='weapons' else '')+'<th>说明</th></tr></thead><tbody>'
            for itemid in ids:
                e=lookup[itemid]
                body+=f'<tr class="entry-card"><td><a class="item-name" href="{itemid}.html">{picture(e)}<span>{E(e["title"])}</span></a></td>'
                if id=='weapons':body+=''.join(f'<td>{E(e["stats"].get(k,"—"))}</td>' for k in ['伤害类型','基础伤害','使用时间'])
                body+=f'<td>'+ (f'<b>{E(e["tag"])}：</b>' if e['tag'] else '')+f'{E(e["summary"])}</td></tr>'
            body+='</tbody></table></div></section>'
        body+='</div>'
    else:
        body+='<div class="cards category-list">'+''.join(card(e) for e in entries)+'</div>'
    body+='<p id="empty-filter" hidden>没有匹配的词条，试试其他关键词。</p>'
    write(c,n,body,c)
for e in ENTRIES:
    body=crumb(CATEGORIES[e['category']])+f'<div class="page-heading"><span class="eyebrow">{E(e["tag"] or CATEGORIES[e["category"]])}</span><h1>{E(e["title"])}</h1><p>{E(e["summary"])}</p></div><div class="article-layout"><article>'
    body+='<nav class="toc" aria-label="本页目录"><b>本页内容</b>'+''.join(f'<a href="#section-{i}">{E(t)}</a>' for i,t in enumerate(e['sections']))+'</nav>'
    if e['id']=='progression':
        body+='<div class="route"><span class="eyebrow">当前主线路线</span>'
        for i,id in enumerate(['wave-guide','wave-duo','written','forest','orochimaru','rain-genin','dosu','gaara']):
            body+=f'<div class="route-step"><span>{i+1:02}</span>{link(id)}<small>'+{'wave-duo':'双首领战','orochimaru':'必经遭遇 · 写轮眼','dosu':'预选赛','gaara':'骷髅王 / 400 生命 · 八门'}.get(id,'剧情推进')+'</small></div>'
        body+='<div class="optional-route">可选分支：'+link('neji')+' → '+link('byakugan')+'<br><small>正式赛阶段开放，不是主线必打。</small></div></div>'
    for i,(heading,value) in enumerate(e['sections'].items()):
        content='<ol>'+''.join(f'<li>{E(v)}</li>' for v in value)+'</ol>' if isinstance(value,list) else '<p>'+E(value)+'</p>'
        body+=f'<section class="article-section" id="section-{i}"><h2>{E(heading)}</h2>{content}</section>'
    body+='<section class="related"><h2>相关词条</h2><div class="related-links">'+''.join(link(id) for id in e['related'])+'</div></section><details class="sources"><summary>内容依据 · 当前源码已核对，实机验证依模组进度</summary><p>数值为源码基础值，不代表各难度下游戏最终值；近期考试与美术仍在打磨。攻略建议不等同于实机验收。</p><ul>'
    for p in e['source']:
        repo=str(source_path(p).relative_to(ROOT))
        body+=f'<li><a href="https://github.com/tuzhechen2005/terraria-naruto-mod/blob/{SHA}/{E(repo)}">{E(repo)}</a></li>'
    body+='</ul><p>依据本地提交 '+SHA[:7]+' 的工作区核对。若该提交尚未推送，上述公开源码链接会在推送后可用。</p></details></article>'
    body+=f'<aside class="infobox"><div class="infobox-title">{E(e["title"])}</div><div class="infobox-art">{picture(e)}</div><div class="infobox-subheading">基本资料</div><dl><dt>分类</dt><dd><a href="{e["category"]}.html">{CATEGORIES[e["category"]]}</a></dd><dt>版本</dt><dd>{E(VERSION)}</dd>'
    for k,v in e['stats'].items(): body+=f'<dt>{E(k)}</dt><dd>{E(v)}</dd>'
    body+='</dl><div class="info-note">属性来自当前源码。装备修饰、角色加成与难度缩放可能改变游戏内数值。</div></aside></div>'
    write(e['id'],e['title'],body,e['category'],e['summary'])
write('about','关于百科',crumb('关于百科')+'''<div class="page-heading"><span class="eyebrow">ABOUT THIS WIKI</span><h1>让资料跟上冒险。</h1><p>Terruto Wiki 为当前模组提供中文玩家资料，内容与网站源码一起保存在模组仓库。</p></div><article class="about"><h2>内容依据</h2><p>本版收录波之国与中忍考试已实现的玩家内容，以及明确标注获取边界的结印忍术开发试用内容。属性从 C# 源码提取，获取方式、流程与机制经过人工核对。设计文档中的未来内容只在开发路线页展示。</p><h2>验证范围</h2><p>已实现、构建通过与游戏内验收是不同状态。本百科不把源码核对视为实机验收；考试篇、联机与近期修改仍依项目验收记录更新。</p><h2>未收录的测试和兼容物品</h2><p>M0 螺旋丸、开发者之翼与测试卷轴属于开发工具。旧版任务卷轴会转为忍者手册，旧版白挑战卷轴会转为再不斩挑战卷轴；它们不作为当前正常获取路线。</p><h2>维护与发布</h2><p>人工说明位于 wiki/content.py；wiki/build.py 提取属性、导出当前贴图并生成 wiki/site。运行核对脚本可检查源码是否在百科整理后变化。新版本数值修改后需要重新核对说明并构建。</p><h2>说明与权利</h2><p>本项目是非官方同人作品，火影角色与设定、Terraria 的相关权利属于各自权利方。百科仅复用本仓库游戏素材，不包含动画原声或官方截图。</p><p><a href="https://github.com/tuzhechen2005/terraria-naruto-mod">查看模组仓库 ↗</a></p></article>''')
(OUT/'source-manifest.json').write_text(json.dumps({'commit':SHA,'version':VERSION,'files':manifest},ensure_ascii=False,indent=2),encoding='utf-8')
print(f'Generated {len(ENTRIES)} entries / {len(list(OUT.glob("*.html")))} pages at {OUT}')
