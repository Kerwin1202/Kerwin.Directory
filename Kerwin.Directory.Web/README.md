

# Kerwin Directory

Kerwin Directory ��һ����Դ��Ŀ¼������.

����Ԥ��һ��~~ 

[http://img.zhanghuanglong.com//image/2018/04/14/20180414213357973963.png](http://img.zhanghuanglong.com//image/2018/04/14/20180414213357973963.png)

û���Ǹ�`directory lister`��࣬�Ҿ��ǰ������д�ģ�����һ��`directory lister`û�ж�ĳ��Ŀ¼����ĳ���ļ����ܵĲ��������Ծ����Լ�д��

## Ŀ¼

* [Windows](#Windows�²���)
    * [IIS����](#IIS����)
        * [��װIIS](#��װIIS)
        * [��װ .net core sdk](#��װ-.net-core-sdk)
        * [����վ��](#����վ��)
        * [����Ӧ�ó����](#����Ӧ�ó����)
    * [Nginx����](#Windows-or-Nginx)
* [Linux](#Linux����)
    * [Nginx����](#Windows-or-Nginx)

## Windows����

### ����Դ��  

Դ���ַ������ʲ��˾ʹ� [Kerwin.Directory](https://git.kerwin.cn/Kerwin/Kerwin.Directory) ���ߵ�[ֻΪСվ](https://www.zhanghuanglong.com/)�� Git�ֿ�����һ��  ��ΪĿǰ�����ַ���˶˿ڣ����첻���˾ͻ��� 23333

[https://git.kerwin.cn:4443/Kerwin/Kerwin.Directory](https://git.kerwin.cn:4443/Kerwin/Kerwin.Directory)

ConfigSettings.cs �ļ��������ļ��������޸ģ�

### IIS����
1. ��װIIS��`��ʼ-->windowsϵͳ-->�������-->����-->���û�ر�windows����`����ѡ`Internet Information Services`�µ����У����ȷ�Ͽ�ʼ��װIIS

![image](http://img.zhanghuanglong.com//image/2018/04/14/20180414205418280261.png)
* ��װ .net core sdk  ����sdk 

    x64 https://www.microsoft.com/net/download/thank-you/dotnet-sdk-2.1.104-windows-x64-installer
    
    x86 https://www.microsoft.com/net/download/thank-you/dotnet-sdk-2.1.104-windows-x86-installer
    
    ��һ����װ����

* ����վ�㣺`�һ��ҵĵ���-->����-->�����Ӧ�ó���-->IIS������` �½�վ�㣬����ͼ  վ��Ŀ¼Ҫָ��wwwroot���ڵ�Ŀ¼

![image](http://img.zhanghuanglong.com//image/2018/04/14/20180414204545110958.png)

* ��������Ӧ�ó����Ϊ���й� ��2����ͼ����վ��һ����һ��Ӧ�ó����,,���Ӧ�ó����-->˫����վ���ƶ�Ӧ��-->.net clr�汾ѡ�����й�

* ����iis���� cmd����������

```
net stop was /y
net start w3svc
```

### Windows-Nginx����

������ʵ��Linux-Nginx����һ�������Լ�ʹ��iis�����ûд�� 23333

### Linux-Nginx����

��UbuntuΪ��
* ��װ.net core sdk

```
root@ubuntu:~# curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
root@ubuntu:~# sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
root@ubuntu:~# sudo sh -c 'echo "deb [arch=amd64] root@ubuntu:~# https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'

root@ubuntu:~# sudo apt-get install apt-transport-https
root@ubuntu:~# sudo apt-get update
root@ubuntu:~# sudo apt-get install dotnet-sdk-2.1.104
```

����Ϊ��װ�ɹ� ��װ֮ǰҲ��������鿴�Ƿ�װ��

```
root@ubuntu:~# dotnet --version
2.1.104
root@ubuntu:~# 

```

* ��װgit

```
root@ubuntu:~# sudo apt-get install git
```

����Ϊ��װ�ɹ� ��װ֮ǰҲ��������鿴�Ƿ�װ��

```
root@ubuntu:~# git --version
git version 2.7.4
root@ubuntu:~# 
```

* clone��Դ��  Դ���ַ������ʲ��˾ʹ� [Kerwin.Directory](https://git.kerwin.cn/Kerwin/Kerwin.Directory) ���ߵ�[ֻΪСվ](https://www.zhanghuanglong.com/)�� Git�ֿ�����һ��  ��ΪĿǰ�����ַ���˶˿ڣ����첻���˾ͻ��� 23333

```
root@ubuntu:~# git clone https://git.kerwin.cn:4443/Kerwin/Kerwin.Directory.git
root@ubuntu:~# cd Kerwin.Directory/
root@ubuntu:~/Kerwin.Directory# ls
Kerwin.Directory.sln  Kerwin.Directory.Web
root@ubuntu:~/Kerwin.Directory# 

```
* ��װNginx

```
root@ubuntu:~/# sudo apt-get install nginx
root@ubuntu:~/# sudo service nginx start
```
* �޸�Nginx���� 

```
root@ubuntu:~/Kerwin.Directory# sudo nano /etc/nginx/sites-available/default

```
û�а�װ��NginxҲ�����ֱ�Ӱ����������滻Ϊ����default
```
server {
    listen        80;
    server_name   example.com *.example.com; #����
    location / {
        proxy_pass         http://localhost:5000;  #���ؼ����ĵ�ַ ����˿���ʱĬ��5000 ������޸�Ҳ���Բ��޸�
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $http_host;
        proxy_cache_bypass $http_upgrade;
    }
}
```
* Nginx���¼�������

```
root@ubuntu:~/Kerwin.Directory# sudo nginx -s reload

```

* �޸�վ�������ļ� ����������ļ��������ö������� ��Щ���ñ�ע��,����δʵ��

```
root@ubuntu:~/Kerwin.Directory# cd Kerwin.Directory.Web/Models/
root@ubuntu:~/Kerwin.Directory/Kerwin.Directory.Web/Models# nano ConfigSettings.cs 

```

�������Բ��޸ģ�Ҳ���԰���ע���޸ģ��ļ��е��������Ҫ�޸İ� `h:\`�޸�Ϊ��Ҫ�����ĸ�Ŀ¼���� ���� `/root/`

```
/// <summary>
/// Root dir
/// ��Ŀ¼ ʵ���ļ����ڵ�Ŀ¼
/// </summary>
public static string RootDir { get; set; } = @"h:\";
```

* ����

```
//�����һ����������������޸����������ڵ�Ŀ¼����ȷ,,��������
root@ubuntu:~/Kerwin.Directory/Kerwin.Directory.Web/Models# cd ../../ 
root@ubuntu:~/Kerwin.Directory# dotnet run -p Kerwin.Directory.Web -c Release --no-launch-profile
Hosting environment: Production
Content root path: /root/Kerwin.Directory/Kerwin.Directory.Web
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.

```

���ڷ�����������Linux���ڵľ������������ɷ�����..

* ���5000�˿ڱ�ռ�û����뻻���˿�

```
root@ubuntu:~/Kerwin.Directory# ASPNETCORE_URLS="http://localhost:52521" dotnet run -p Kerwin.Directory.Web -c Release --no-launch-profile

```

