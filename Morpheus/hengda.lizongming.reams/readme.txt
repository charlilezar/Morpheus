abp new HengDa.LiZongMing.REAMS -t app -ui blazor --separate-identity-server


sso.hengda.show
/usr/share/dotnet/dotnet

/www/wwwroot/reams/HengDa.LiZongMing.REAMS.IdentityServer
/www/wwwroot/reams/HengDa.LiZongMing.REAMS.IdentityServer/HengDa.LiZongMing.REAMS.IdentityServer.dll --urls=https://*:44300

ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_Kestrel__Certificates__Default__Password=123456
ASPNETCORE_Kestrel__Certificates__Default__Path=/www/server/aspnetapp.pfx


reams.hengda.show
/usr/share/dotnet/dotnet

/www/wwwroot/reams/HengDa.LiZongMing.REAMS.HttpApi.Host
/www/wwwroot/reams/HengDa.LiZongMing.REAMS.HttpApi.Host/HengDa.LiZongMing.REAMS.HttpApi.Host.dll --urls=https://*:44300



如果swagger认证时出现
TypeError: Failed to fetch

那是因为必须用https且有效的证书签名。

pem转pfx(不带ca证书）
https://www.cnblogs.com/youjianjiangnan/p/12828208.html
以test.pem转test.pfx为例

cp  /www/server/panel/vhost/cert/no-site.cn/fullchain.pem hengda.show.pem
cp  /www/server/panel/vhost/cert/no-site.cn/privkey.pem   hengda.show.key
#openssl rsa -in hengda.show.pem -out hengda.show.key
openssl x509 -in hengda.show.pem -out hengda.show.crt
openssl pkcs12 -export -out hengda.show.pfx -inkey hengda.show.key -in hengda.show.crt
然后输入密码 123456

linux系统信任证书
sudo cp 证书路径.crt /usr/local/share/ca-certificates
sudo cp /www/server/hengda.show.crt /usr/local/share/ca-certificates
sudo update-ca-certificates