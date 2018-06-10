# dme
dist model engine，主要用于支撑空间模型、算法的构建、运算。


## 版本迭代

* **1.0.0-SNAPSHOT**

 
>  [2018.03.01]  
1. 数据库设计
2. 基础框架搭建；
3. 平台接口定义；

>  [2018.04.01]  
1. 提供基础服务；
2. 提供分布式缓存服务；
2. 集成webapi和swagger api；

>  [2018.05.01]  
1. 提供模型、算法注册；
2. 提供模型、算法、数据源信息查询；

>  [2018.06.05]  
1. 定义分布式文件系统接口；
2. 提供分布式文件服务（mongodb）；
3. 定义数据集；

>  [2018.06.06]  
1. 删除模块：Dist.Dme.AECommon；
2. 添加模块：Dist.Dme.SRCE，空间关系计算引擎，主要是想抽象一个独立空间计算层，屏蔽不同的平台差异，如：超图、esri等；

>  [2018.06.09]  
1. 添加消息模块：Dist.Dme.HSMessage，意在提供高速消息服务，包括MQ、短信或者邮箱消息等等；

>  [2018.06.10]  
1. 添加压盖分析算法，以及相关空间分析辅助类；
