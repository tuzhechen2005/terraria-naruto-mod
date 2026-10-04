#!/usr/bin/env python3
"""校验生成页、内部链接、图片、锚点，以及百科依赖源码的新鲜度。"""
import hashlib,json,sys
from pathlib import Path
from html.parser import HTMLParser
from urllib.parse import urlsplit,unquote
ROOT=Path(__file__).resolve().parents[1]
SITE=ROOT/'wiki/site'
errors=[]
class Page(HTMLParser):
    def __init__(self,text):
        super().__init__();self.refs=[];self.ids=set();self.feed(text)
    def handle_starttag(self,tag,attrs):
        attrs=dict(attrs)
        if 'id' in attrs:self.ids.add(attrs['id'])
        for k in ['href','src']:
            if k in attrs:self.refs.append(attrs[k])
pages={p:Page(p.read_text()) for p in SITE.glob('*.html')}
for p,parsed in pages.items():
    for ref in parsed.refs:
        url=urlsplit(ref)
        if url.scheme or url.netloc:continue
        target=(p.parent/unquote(url.path)).resolve() if url.path else p
        if not target.exists():errors.append(f'{p.name}: missing {ref}')
        elif url.fragment and target in pages and unquote(url.fragment) not in pages[target].ids:errors.append(f'{p.name}: missing anchor {ref}')
manifest=json.loads((SITE/'source-manifest.json').read_text())
changed=[]
for path,expected in manifest['files'].items():
    file=ROOT/path
    if not file.exists() or hashlib.sha256(file.read_bytes()).hexdigest()!=expected:changed.append(path)
for error in errors:print('ERROR',error)
for path in changed:print('STALE',path)
print(f'{len(pages)} pages, links / assets / anchors: {"PASS" if not errors else "FAIL"}; {len(changed)} changed source files')
sys.exit(1 if errors or changed else 0)
