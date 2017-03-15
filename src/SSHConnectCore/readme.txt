
http://askubuntu.com/questions/677773/transfer-file-to-windows-server-from-ubuntu-with-rsync

This might be needed?
	sudo apt-get install cifs-utils

	sudo mkdir /media/BACKUP
I found that i needed to add password
	sudo mount -t cifs -o username=domainusername,password=yourpassword //ip_add/ShareFolder /media/BACKUP

	sudo gedit /etc/fstab
Couldnt get .smb credentials to work so i went with this
	//ip_add/ShareFolder /media/BACKUP/ cifs username=domainusername,password=yourpassword,iocharset=utf8,sec=ntlm 0 0

Might need to add firewall rule for local ip to remote ip