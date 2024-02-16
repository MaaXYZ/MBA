> [!IMPORTANT]  
> 由于开发者是个单线程生物，在 MaaFramework 的一些基础设施可用之前，此仓库中断维护（咕了）。
> 
> Since the developer is a single-threaded creature, this repository is in hiatus until some of the MaaFramework infrastructure is available.

<div align="center">
<img alt="LOGO" src="./assets/logo.ico" width="256" height="256" />

# MBA

基于 MAA 全新架构的 BA 小助手

图像技术 + 模拟控制，解放双手，不再点点点！

由 [MaaFramework](https://github.com/MaaAssistantArknights/MaaFramework) 强力驱动！

</div>

## 下载地址

- [正式版](https://github.com/MaaAssistantArknights/MBA/releases/latest)
- [预览版](https://github.com/MaaAssistantArknights/MBA/actions/workflows/ci.yml)

### 注意

- 打不开 / 一闪而过可能是存在中文路径。
- 想要开箱即用，请直接下载 `MBA.Cli-single-cut` 包。
- `runtime-relied` 包在运行前需要预先安装 [.NET 运行时 7.0](https://dotnet.microsoft.com/zh-cn/download/dotnet/7.0)
- 其他系统、架构想要下载预构建好的 `runtime-relied` 包，欢迎 pr 或提 issue。

## 使用说明

### 基本说明

1. 请根据 [模拟器支持情况](https://maa.plus/docs/1.3-模拟器支持.html)，进行对应的操作。
2. 修改模拟器分辨率为 `16:9` 比例，最低 `1280 * 720`，更高不限。
3. 开始使用吧！

## 开发相关

- `tools/CropRoi` 可以用来裁剪图片和获取 ROI
- Pipeline 协议请参考 [MaaFramework 的文档 (Document of MaaFramework)](https://github.com/MaaAssistantArknights/MaaFramework/blob/main/docs/zh_cn/3.3-%E4%BB%BB%E5%8A%A1%E6%B5%81%E6%B0%B4%E7%BA%BF%E5%8D%8F%E8%AE%AE.md)
- [多语言支持](./assets/README.md#多语言支持)
- [Multi-language support](./assets/README.md#multi-language-support)

## Join us
用户反馈 / 开发 QQ 群：205135027

## How to build

0. Clone 本仓库

1. 编译环境准备

    下载 [.NET 7.0 SDK](https://dotnet.microsoft.com/zh-cn/download/dotnet/7.0) 或 Visual Studio 2022

2. Build

    - 使用 VS 生成

    - 使用命令行生成

    ```sh
    cd MBA/src/MBA.Cli
    dotnet build
    # 或专为安装了运行时的 winX64 用户打包
    dotnet build -r win-x64 -c Release -p:PublishSingleFile=true --self-contained false
    # 或者发布一个自包含应用
    dotnet publish -r win-x64 -c Release -p:PublishSingleFile=true --self-contained true -p:PublishTrimmed=true
    ```

3. 生成的二进制及相关资源文件在 `MBA\src\MBA.Cli\bin` 目录下

## 致谢

### 开源库

- [MaaFramework](https://github.com/MaaAssistantArknights/MaaFramework)：自动化测试框架
- [MaaAgentBinary](https://github.com/MaaAssistantArknights/MaaAgentBinary): Maa 代理的预构建文件
- [MaaToolKit.Extensions](https://github.com/moomiji/MaaToolKit.Extensions)：MaaFramework 的 C# 包装
- [Serilog](https://github.com/serilog/serilog)：C# 日志记录库

## 画大饼

### v1.0

- [ ] Daily

- Home
    - [x] Tasks
    - [x] Mailbox
    - [x] Cafe : TODO 需要缩放
    - [ ] Lesson : 需要判断颜色 + 遍历所有可选项
    - [x] Club
    - [ ] Crafting : 需要判断颜色 + 遍历所有可选项
    - [ ] Shop
- Campaign
    - [x] Tactical Challenge
    - [x] Scrimmage
    - [x] Commissions
    - [x] Bounty
- Mission
    - [ ] Normal
    - [ ] Hard
    - [ ] Event

- [ ] Weekly

- [ ] Support SC / TC / EN / JP

### v2.0

- [ ] 关卡掉落识别
- [ ] 仓库识别
- [ ] 材料需求计算
- [ ] 活动关卡导航

### v3.0

- [ ] 自动活动导航
- [ ] 自动寻路 (只限无迷雾地图)
- [ ] 自动战斗 (不凹极限比 Auto 打的好就行)
