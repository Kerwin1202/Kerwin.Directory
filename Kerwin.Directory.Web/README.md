

# Kerwin Directory

Kerwin Directory 是一个开源的目录管理工具.

先来预览一波~~ 

[http://img.zhanghuanglong.com//image/2018/04/14/20180414213357973963.png](http://img.zhanghuanglong.com//image/2018/04/14/20180414213357973963.png)

没错是跟`directory lister`差不多，我就是按照这个写的，看了一下`directory lister`没有对某个目录或者某个文件加密的操作，所以就想自己写个

## 目录

* [Windows](#Windows下部署)
    * [IIS部署](#IIS部署)
        * [安装IIS](#安装IIS)
        * [安装 .net core sdk](#安装-.net-core-sdk)
        * [配置站点](#配置站点)
        * [设置应用程序池](#设置应用程序池)
    * [Nginx部署](#Windows-or-Nginx)
* [Linux](#Linux部署)
    * [Nginx部署](#Windows-or-Nginx)

## Windows部署

### 下载源码  

源码地址如果访问不了就打开 [Kerwin.Directory](https://git.kerwin.cn/Kerwin/Kerwin.Directory) 或者到[只为小站](https://www.zhanghuanglong.com/)里 Git仓库搜索一下  因为目前这个地址带了端口，改天不高兴就换了 23333

[https://git.kerwin.cn:4443/Kerwin/Kerwin.Directory](https://git.kerwin.cn:4443/Kerwin/Kerwin.Directory)

ConfigSettings.cs 文件是配置文件，自行修改，

### IIS部署
1. 安装IIS：`开始-->windows系统-->控制面板-->程序-->启用或关闭windows功能`，勾选`Internet Information Services`下的所有，点击确认开始安装IIS

![image](http://img.zhanghuanglong.com//image/2018/04/14/20180414205418280261.png)
* 安装 .net core sdk  下载sdk 

    x64 https://www.microsoft.com/net/download/thank-you/dotnet-sdk-2.1.104-windows-x64-installer
    
    x86 https://www.microsoft.com/net/download/thank-you/dotnet-sdk-2.1.104-windows-x86-installer
    
    下一步安装即可

* 配置站点：`右击我的电脑-->管理-->服务和应用程序-->IIS管理器` 新建站点，如下图  站点目录要指向wwwroot所在的目录

![image](http://img.zhanghuanglong.com//image/2018/04/14/20180414204545110958.png)

* 设置设置应用程序池为无托管 第2步骤图中网站上一个有一个应用程序池,,点击应用程序池-->双击网站名称对应的-->.net clr版本选择无托管

* 重启iis服务 cmd下输入以下

```
net stop was /y
net start w3svc
```

### Windows-Nginx部署

部署其实跟Linux-Nginx部署一样，我自己使用iis部署就没写了 23333

### Linux-Nginx部署

以Ubuntu为例
* 安装.net core sdk

```
root@ubuntu:~# curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
root@ubuntu:~# sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
root@ubuntu:~# sudo sh -c 'echo "deb [arch=amd64] root@ubuntu:~# https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'

root@ubuntu:~# sudo apt-get install apt-transport-https
root@ubuntu:~# sudo apt-get update
root@ubuntu:~# sudo apt-get install dotnet-sdk-2.1.104
```

以下为安装成功 安装之前也可以输入查看是否安装过

```
root@ubuntu:~# dotnet --version
2.1.104
root@ubuntu:~# 

```

* 安装git

```
root@ubuntu:~# sudo apt-get install git
```

以下为安装成功 安装之前也可以输入查看是否安装过

```
root@ubuntu:~# git --version
git version 2.7.4
root@ubuntu:~# 
```

* clone下源码  源码地址如果访问不了就打开 [Kerwin.Directory](https://git.kerwin.cn/Kerwin/Kerwin.Directory) 或者到[只为小站](https://www.zhanghuanglong.com/)里 Git仓库搜索一下  因为目前这个地址带了端口，改天不高兴就换了 23333

```
root@ubuntu:~# git clone https://git.kerwin.cn:4443/Kerwin/Kerwin.Directory.git
root@ubuntu:~# cd Kerwin.Directory/
root@ubuntu:~/Kerwin.Directory# ls
Kerwin.Directory.sln  Kerwin.Directory.Web
root@ubuntu:~/Kerwin.Directory# 

```
* 安装Nginx

```
root@ubuntu:~/# sudo apt-get install nginx
root@ubuntu:~/# sudo service nginx start
```
* 修改Nginx配置 

```
root@ubuntu:~/Kerwin.Directory# sudo nano /etc/nginx/sites-available/default

```
没有安装过Nginx也不会的直接把下面内容替换为整个default
```
server {
    listen        80;
    server_name   example.com *.example.com; #域名
    location / {
        proxy_pass         http://localhost:5000;  #本地监听的地址 这个端口暂时默认5000 下面会修改也可以不修改
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $http_host;
        proxy_cache_bypass $http_upgrade;
    }
}
```
* Nginx重新加载配置

```
root@ubuntu:~/Kerwin.Directory# sudo nginx -s reload

```

* 修改站点配置文件 这个是配置文件所有配置都在里面 有些配置被注释,代表未实现

```
root@ubuntu:~/Kerwin.Directory# cd Kerwin.Directory.Web/Models/
root@ubuntu:~/Kerwin.Directory/Kerwin.Directory.Web/Models# nano ConfigSettings.cs 

```

其他可以不修改，也可以按照注释修改，文件中的这个必须要修改把 `h:\`修改为想要监听的根目录即可 例如 `/root/`

```
/// <summary>
/// Root dir
/// 根目录 实际文件所在的目录
/// </summary>
public static string RootDir { get; set; } = @"h:\";
```

* 运行

```
//下面第一步这个是由于上面修改了配置所在的目录不正确,,向上两个
root@ubuntu:~/Kerwin.Directory/Kerwin.Directory.Web/Models# cd ../../ 
root@ubuntu:~/Kerwin.Directory# dotnet run -p Kerwin.Directory.Web -c Release --no-launch-profile
Hosting environment: Production
Content root path: /root/Kerwin.Directory/Kerwin.Directory.Web
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.

```

现在访问域名或者Linux所在的局域网机器即可访问了..

* 如果5000端口被占用或者想换个端口

```
root@ubuntu:~/Kerwin.Directory# ASPNETCORE_URLS="http://localhost:52521" dotnet run -p Kerwin.Directory.Web -c Release --no-launch-profile

```

