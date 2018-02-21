[![Build Status](https://travis-ci.org/patstovepipe/sshconnectcore.svg?branch=master)](https://travis-ci.org/patstovepipe/sshconnectcore)

# sshconnectcore

Website / API to manage files/roms on a [retropie](https://retropie.org.uk/) install.
This application can then be hosted on a windows/linux server separate from the retropie.

If using a windows server then follow instructions [here](https://askubuntu.com/questions/677773/transfer-file-to-windows-server-from-ubuntu-using-rsync).
From the instructions I found that I needed to add a password

    sudo mount -t cifs -o username=domainusername,password=yourpassword //ip_add/ShareFolder /media/BACKUP
    
I couldn't get the smb credentials working properly so I added this in the fstab file.

   	//ip_add/ShareFolder /media/BACKUP/ cifs username=domainusername,password=yourpassword,iocharset=utf8,sec=ntlm 0 0

The following package might need to be installed to get the windows file share working properly.

    sudo apt-get install cifs-utils
    
Windows firewall rule might also be needed (for traffic between windows server and retropie).
