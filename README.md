
MVVM-Sidekick MVVM密友
===================

轻量级MVVM框架 

目的是集合Reactive Command, Prism 等框架的优点，应对.net 4.5 和 Windows Runtime带来的变化，为新技术环境量身打造一套以ViewModelBase/ReactiveCommand为核心的基础。

从设计开始就以Windows 8 Style App作为运行环境进行测试，野心覆盖所有XAML运行环境。


功能特色
======

1 全面支持DataContract序列化 可以将一个VM的全部状态用任何方式保存为JSon/XML 而不必担心反序列化后部分机制停摆。

2 轻量级代码级框架，不必安装全部DLL或者引用工程，只需要将指定代码文件加入你的工程切安装Reactive Extension就可以用。

3 ViewModel所有的成员之间用事件序列驱动交互，只需要用 Linq-Like 语法进行配置和订阅，订阅在VM 销毁时自动取消。

4 可以在声明property的代码处配置property的业务细节，可以在声明command的代码处配置command的业务细节

5 可以将ViewModel的业务细节[配置与ViewModel的创建时机分离，不但可以在实体外用装饰模式进行批量配置，也可以根据需要临时装饰增加VM的功能。


性能亮点
========

1 对于属性访问采用可内联的直接寻址方式访问提高速度。

2 对于属性名的键值访问采用访问器字典的方式减少内存消耗。



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


