#!/bin/bash

# Aktualisieren der Installationsquellen für unseren Ubuntu-Server
apt-get update

# Installieren der Systemvoraussetzungen von mscs
apt-get -y install default-jre perl libjson-perl python make wget rdiff-backup rsync socat iptables git

# Mscs herunterladen und eigentlichen Installation starten
cd /tmp
git clone https://github.com/MinecraftServerControl/mscs.git
cd mscs
make install

# Minecraft-Welt anlegen
WORLD_NAME="arm"
mscs create $WORLD_NAME 25565
cd /opt/mscs/worlds/$WORLD_NAME

# Spieleinstellungen auf kreativ und flache Welt setzen
echo "gamemode=1" | tee -a server.properties
echo "level-type=FLAT" | tee -a server.properties

# Lizenzbedingungen akzeptieren
echo "#By changing the setting below to TRUE you are indicating your agreement to our EULA (https://account.mojang.com/documents/minecraft_eula)." | sudo tee eula.txt
echo "#$(date)" | tee -a eula.txt
echo "eula=true" | tee -a eula.txt

mscs start $WORLD_NAME
