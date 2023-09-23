'''
检查目标
1. 检查 Sub 型任务
    必须含有键值对 "is_sub": true
2. 检查含有键值对 "is_sub": true 的任务
    必须是 Sub 型任务
3. 检查含有键键值对 "recognition": "OCR" 的任务
    必须含有键 "text"
4. 检查含有键 "text" 的任务
    （非通用资源）必须在 其他语言 资源中也含有
    （非通用资源）必须是 Partial 型任务
5. 检查含有键键值对 "recognition": "TemplateMatch" 的任务
    必须含有键 "template"
6. 检查含有键 "template" 的任务
    （非通用资源）必须在 其他语言 资源中也含有
    （非通用资源）必须是 Partial 型任务
7. 检查 Base 和 其他语言 资源中的 Partial 型任务 
    必须互为拥有
8. 检查 Goto 型任务的父级任务
    next 必须含有 Sub_Wait_NowLoading_Partial
9. 检查 其他语言 资源中相同文件名的任务
    必须和 EN 中相同文件名的任务一一对应
'''

import os
import sys
import json

pipeline_path = 'assets/Resource/{}/pipeline'
base_tag = 'Base'
lang_tags = ['EN', 'JP', 'KR', 'TH', 'TC', 'SC'] # 基准资源放第 0 位
skip_key = 'skip_review'

def load_json_files(directory: str) -> (dict, dict, bool):
    datas = {}
    merged_data = {}
    failed = False
    try:
        for filename in os.listdir(directory):
            if filename.endswith('.json'):
                path = f'{directory}/{filename}'
                with open(path, 'r', encoding='utf-8') as f:
                    data = json.load(f)
                    datas[path] = data          # 将 path 和 data 添加到 datas
                    merged_data.update(data)    # 合并 data
    except Exception as e:
        print(f'::warning:: title=读取 {directory} 时出现错误::{e}')
        failed = True
    return (datas, merged_data, failed)

def review_Sub(datas: dict) -> int:
    rule = 1
    print(f'::notice::Rule: {rule}')
    err = 0
    for path, data in datas.items():
        for k, v in data.items():
            if v.get(skip_key) == rule:
                continue
            if k.upper().startswith('SUB') and not v.get('is_sub'):
                print(f'::error file={path},title={k}::必须含有键值对 "is_sub": true')
                err += 1
    return err

def review_is_sub(datas: dict) -> int:
    rule = 2
    print(f'::notice::Rule: {rule}')
    err = 0
    for path, data in datas.items():
        for k, v in data.items():
            if v.get(skip_key) == rule:
                continue
            if v.get('is_sub') and not k.upper().startswith('SUB'):
                print(f'::error file={path},title={k}::必须是 Sub 型任务')
                err += 1
    return err

def review_OCR(datas: dict) -> int:
    rule = 3
    print(f'::notice::Rule: {rule}')
    err = 0
    for path, data in datas.items():
        for k, v in data.items():
            if v.get(skip_key) == rule:
                continue
            if v.get('recognition', '').upper() == 'OCR' and not v.get('text'):
                print(f'::error file={path},title={k}::必须含有键 "text"')
                err += 1
    return err

def review_text(datas: dict, lang: dict, lang_tag: str) -> int:
    rule = 4
    print(f'::notice::Rule: {rule}')
    err = 0
    for path, data in datas.items():
        for k, v in data.items():
            if v.get(skip_key) == rule:
                continue
            if v.get('recognition', '').upper() != 'OCR':
                continue
            if not k.upper().endswith('PARTIAL'):
                print(f'::error file={path},title={k}::必须是 Partial 型任务')
                err += 1
            if not lang.get(k):
                print(f'::error file={path},title={k}::必须在 {lang_tag} 资源中也含有任务')
                print(f'::error file={path},title={k}::必须在 {lang_tag} 资源中也含有键 "text"')
                err += 2
            elif not lang[k].get('text'):
                print(f'::error file={path},title={k}::必须在 {lang_tag} 资源中也含有键 "text"')
                err += 1
    return err

def review_TemplateMatch(datas: dict) -> int:
    rule = 5
    print(f'::notice::Rule: {rule}')
    err = 0
    for path, data in datas.items():
        for k, v in data.items():
            if v.get(skip_key) == rule:
                continue
            if v.get('recognition', '').upper() == 'TEMPLATEMATCH' and not v.get('template'):
                print(f'::error file={path},title={k}::必须含有键 "template"')
                err += 1
    return err

def review_template(datas: dict, lang: dict, lang_tag: str) -> int:
    rule = 6
    print(f'::notice::Rule: {rule}')
    err = 0
    for path, data in datas.items():
        for k, v in data.items():
            if v.get(skip_key) == rule:
                continue
            if v.get('recognition', '').upper() != 'TEMPLATEMATCH':
                continue
            if not k.upper().endswith('PARTIAL'):
                print(f'::error file={path},title={k}::必须是 Partial 型任务')
                err += 1
            if not lang.get(k):
                print(f'::error file={path},title={k}::必须在 {lang_tag} 资源中也含有任务')
                print(f'::error file={path},title={k}::必须在 {lang_tag} 资源中也含有键 "template"')
                err += 2
            elif not lang[k].get('template'):
                print(f'::error file={path},title={k}::必须在 {lang_tag} 资源中也含有键 "template"')
                err += 1
    return err

def review_Partial(datas: dict, lang: dict, lang_tag: str) -> int:
    rule = 7
    print(f'::notice::Rule: {rule}')
    err = 0
    for path, data in datas.items():
        for k, v in data.items():
            if v.get(skip_key) == rule:
                continue
            if k.upper().endswith('PARTIAL') and not lang.get(k):
                print(f'::error file={path},title={k}::必须在 {lang_tag} 资源中也含有任务')
                err += 1
    return err

def review_Goto(datas: dict) -> int:
    rule = 8
    print(f'::notice::Rule: {rule}')
    err = 0
    for path, data in datas.items():
        for k, v in data.items():
            if v.get(skip_key) == rule:
                continue
            for task in v.get("next", []):
                if task == 'Sub_Wait_NowLoading_Partial':
                    break
                if 'GOTO' in task.upper():
                    print(f'::error file={path},title={k}::{task} 的前面必须含有 Sub_Wait_NowLoading_Partial')
                    err += 1
                    break
    return err

def review_TaskName(datas: dict, langs: dict, lang_tag: str) -> int:
    rule = 9
    print(f'::notice::Rule: {rule}')
    err = 0
    same_filename_list = []
    for path in datas:
        for lpath in langs:
            if os.path.basename(path) in lpath:
                same_filename_list.append((path, lpath))
    for path, lpath in same_filename_list:
        tasks, ltasks = list(datas[path].keys()), list(langs[lpath].keys())
        lentasks, lenltasks = len(tasks), len(ltasks)
        for i in range(min(lentasks, lenltasks)):
            task, ltask = tasks[i], ltasks[i]
            if task != ltask:
                print(f'::error file={lpath},title={ltask}::必须和 {lang_tag} 的任务 {task} 一一对应')
                err += 1
                break
        if lentasks != lenltasks:
            print(f'::error file={lpath},title=任务数量不一致::任务数量 {lenltasks} 必须和 {lang_tag} 的任务数量 {lentasks} 一致')
            err += 1
    return err

review_Bases = [
    review_Sub, # 1
    review_is_sub, # 2
    review_OCR, # 3
    review_TemplateMatch, # 5
    review_Goto, # 8
]

review_Bases_in_Lang = [
    review_text, # 4
    review_template, # 6
    review_Partial, #7
]

review_Langs_in_Base = [
    review_Partial, #7
]

review_Basics_in_Langs = [
    review_TaskName, #9
]

ret = 0
os.chdir('../../')

Bases, Base, Failed = load_json_files(pipeline_path.format(base_tag))
if Failed:
    print(f'::error::读取 {base_tag} 失败')
    sys.exit(1)

for review in review_Bases:
    ret += review(Bases)

for lang_tag in lang_tags:
    Langs, Lang, Failed= load_json_files(pipeline_path.format(lang_tag))
    if Failed:
        continue
    
    for review in review_Bases_in_Lang:
        ret += review(Bases, Lang, lang_tag)
    for review in review_Langs_in_Base:
        ret += review(Langs, Base, base_tag)
    
    if lang_tag is lang_tags[0]:
        Basics, Basic = Langs, Lang
        continue
    for review in review_Basics_in_Langs:
        ret += review(Basics, Langs, lang_tags[0])

print(f'::warning::sensei 还剩 {ret} 个错误就完成了 ٩(๑>◡<๑)۶ ')
sys.exit(ret)
