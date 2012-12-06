
MVVM-Sidekick MVVM密友
===================

轻量级MVVM框架 

目的是集合Reactive Command, Prism 等框架的优点，应对.net 4.5 和 Windows Runtime带来的变化，为新技术环境量身打造一套以ViewModelBase/ReactiveCommand为核心的基础。

从设计开始就以Windows 8 Style App作为运行环境进行测试，野心覆盖所有XAML运行环境。



Table-Game-Sidekick
===================

使用MVVM-Sidekick制作的第一个APP，桌游密友.

本意是一个桌游计分器，第一步是实现大富翁计分器，支持扩展开发别的游戏的计分/保存/分享。

可以作为框架使用的例子代码。可是开发进度。。。坑。。。




如何使用MVVMSidekick项目模板？
===================


1 进入MVVMSidekick目录

2 双击 MVVMSidekickVSIX.vsix

3 按照提示安装

4 创建新项目，在c#项目中找到 MVVMSidekick 模板

5 在创建好的项目中用MVVM Sidekick 代码模板 创建DataModel和View/ViewModel文件




另：请自行倒入 MVVMSidekick\CommonCode 下的MVVM.snippet文件

或者使用Nuget  键入 

Install-Package MVVMSidekick.Snippet 

支持如下常用代码块：


propvm  	在MVVMSidekick Binable/ViewModel 中增加属性

vmcmd 		在MVVMSidekick Binable/ViewModel 中增加命令


