# Nginx

Setup Nginx to server webgl files and to act as a reverse proxy for websocket. This will allow Nginx to handle the SSL communication so that your game server doesn't have to

## Nginx Setup

First install nginx
```sh
sudo apt update
sudo apt install nginx
```

Copy config from `http.config` locally to default site on server

If you want to host the webgl files on the same VPS then use `http_with_webgl_host.config` instead of `http.config`

```sh
scp -r "http.config" "<serverAddress>:~"
ssh <serverAddress> 'sudo mv ~/http.config /etc/nginx/sites-available/default'
ssh <serverAddress> 'sudo systemctl restart nginx'
```
make sure to replace `<serverAddress>` with the ip address of the serve

### webgl hosting

to correctly host the webgl files, make sure too check the guide here:
[Server configuration code samples](https://docs.unity3d.com/Manual/webgl-server-configuration-code-samples.html). 
The values for Nginx should already be in example [http.config](./http.config) files 

## SSL 

instructions from https://certbot.eff.org/instructions?ws=nginx&os=ubuntufocal&tab=standard

Below is set up for **Ubuntu 20 only** if you are no using this, check the certbot link above for your setup

setup for Ubuntu 20
```sh
sudo snap install core; sudo snap refresh core
sudo snap install --classic certbot

sudo ln -s /snap/bin/certbot /usr/bin/certbot

# will edit nginx config automatically
# run sudo certbot certonly --nginx to avoid editing config
sudo certbot --nginx
```

Test auto renew
```sh
sudo certbot renew --dry-run
```
