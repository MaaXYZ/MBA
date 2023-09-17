# About

## 任务编写标准

| 任务类型 | 任务名 | 任务内容 |
| :----: | :----: | :----: |
| Partial | + _Partial | 使用了多语言资源 |
| Sub | Sub_ + | 子任务 |
| Done | Done_ + | 任务链出口 |
| Entry | 跟随自然语言 | 任务链入口 |
| Start (≈Flag) | Start_EntryName / Start_跟随自然语言  | 某一实际业务入口 |
| End | End_EntryName / End_跟随自然语言 | 某一实际业务出口 |
| Goto | Goto_EntryName | 前往某一实际业务页面 |
| Action | 动作类型_动作对象 [ _动作对象的对象 / _EntryName ] | 实际业务 |
| Flag | 跟随自然语言 + Flag | 遇到 Flag 时结束 |

## 多语言支持

嘤嘤嘤，开发者不懂 `韩语` 和 `泰语`，如有需要，欢迎 pr。

### 相关指南

`assets/Resource` 为 `MaaFramework` 使用的资源文件，其中 `BASE` 为各服务器、各语言的通用资源。

各区域资源文件夹以 `EN` 英语区域为基准，文件夹名分别是：
- 日语: JP
- 韩语: KR
- 英语: EN (Base)
- 泰语: TH
- 繁体中文: TC
- 简体中文: SC

如果要新建一个区域资源文件夹，需要包含 `image/` `pipeline/` `model/` `properties.json`。
- image: 需要通过 `tools/CropRoi` 截图替换
- pipeline: 需要修改 `OCRPartialTasks.json`
- model: 需要添加 [OCR ONNX 模型](https://github.com/MaaAssistantArknights/MaaCommonAssets/tree/main/OCR)
- properties.json (TODO): 需要设置 `"is_base": false, "version": "XX", "EN_version": "XX"`

## Multi-language support

Developer (@Dissectum) totally doesn't understand Korean/KR (한국어) or Thai/TH (ภาษาไทย). Welcome to pull requests if you need it.

### Related Guidelines

`assets/Resource` is the resource file used by `MaaFramework`, where `BASE` is the common resource for each server and each language.

The resource folders for each region are based on the `EN` English region, and the folder names are as follows:
- Japanese: JP
- Korean: KR
- English: EN (Base)
- Thai: TH
- Traditional Chinese: TC
- Simplified Chinese: SC

To create a new region resource folder, you need to include `image/` `pipeline/` `model/` `properties.json`.
- image: need to be replaced by `tools/CropRoi` screenshot.
- pipeline: need to modify `OCRPartialTasks.json`.
- model: need to add [OCR ONNX model](https://github.com/MaaAssistantArknights/MaaCommonAssets/tree/main/OCR).
- properties.json (TODO): need to set `"is_base": false, "version": "XX", "EN_version": "XX"`
