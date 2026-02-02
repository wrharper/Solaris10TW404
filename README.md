# Solaris 10 TalesWeaver Server Setup Guide

## Overview
Complete setup instructions for installing and configuring a fresh Solaris 10 VM to run the TalesWeaver game server with ENDRE database.

---

## Phase 1: Solaris 10 Fresh Install

### Step 1.1: VMware VM Creation
- Create new Solaris 10 VM in VMware Workstation
- Allocate adequate disk space (at least 50GB)
- Set network adapter to NAT (VMnet8)

### Step 1.2: Solaris 10 Installation
During the Solaris 10 GUI installer:

- **Network Configuration**: Static IP
  - IP Address: `10.0.0.249`
  - Subnet Mask: `255.255.255.0`
  - Choose "No" to DHCP
  - Name Service: `none`

- **System Configuration**
  - Hostname: `TWServer` (or your preferred name)
  - Root Password: (choose and remember this)

- **Language Packs**
  - English (POSIX): YES (default)
  - Japanese (EUC): YES
  - Japanese (Shift-JIS): YES

- **Other Settings**: Accept defaults

### Step 1.3: Reboot and Verify Network
After installation completes and Solaris boots:

```bash
ifconfig e1000g0
```

Should show: `inet 10.0.0.249 netmask ffffff00 broadcast 10.0.0.255`

---

## Phase 2: Windows Environment Setup

### Step 2.1: Clear Old Host Key (if reinstalling)
From Windows PowerShell:

```powershell
ssh-keygen -R 10.0.0.249
```

### Step 2.2: Generate SSH Key Pair
From Windows PowerShell:

```powershell
ssh-keygen -t rsa -N "" -f "$env:USERPROFILE\.ssh\id_rsa"
```

### Step 2.3: Test SSH Connection
From Windows PowerShell (accept the new host key):

```powershell
ssh -oKexAlgorithms=diffie-hellman-group1-sha1 -oHostKeyAlgorithms=ssh-rsa -oMACs=hmac-md5,hmac-sha1 -oPreferredAuthentications=password root@10.0.0.249
```

Type `yes` when prompted. Then `exit` to close the connection.

### Step 2.4: Update SSH Environment Configuration
Run this in PowerShell:

```powershell
mkdir $env:USERPROFILE\.ssh -Force
Set-Content -Path "$env:USERPROFILE\.ssh\config" -Value "Host 10.0.0.249`n    KexAlgorithms diffie-hellman-group1-sha1`n    HostKeyAlgorithms ssh-rsa`n    MACs hmac-md5,hmac-sha1`n    PreferredAuthentications password" -Encoding ASCII
icacls "$env:USERPROFILE\.ssh\config" /inheritance:r /grant:r "$env:USERNAME`:F"
```

This enables SSH connections to the old Solaris 10 SSH server.

### Step 2.5: Setup Tera Term for SSH Access
Download and install Tera Term from: https://ttssh2.osdn.jp/

Once installed, configure for Solaris 10:
1. Open Tera Term
2. File → New connection
3. Host: `10.0.0.249`, Port: `22`, SSH selected, OK
4. When prompted, accept the host key
5. Login: `root`, Password: (your root password)
6. Go to **Setup → SSH → Authentication**
   - Key Exchange: `diffie-hellman-group1-sha1`
   - Host Key Algorithm: `ssh-rsa`
   - MAC: `hmac-md5` or `hmac-sha1`
   - Click OK and save the profile

### Step 2.6: Setup Bash Shell and Profile (via Tera Term)
Connect via Tera Term, then run these commands in the Tera Term console:

```bash
usermod -s /usr/bin/bash root
echo "
export PATH=/opt/csw/bin:/usr/sfw/bin:/usr/sbin:/usr/bin:/usr/openwin/bin:/usr/dt/bin:/usr/ccs/bin:$PATH
export PS1=\"[\[\e[35;40m\]\u\[\e[0m\]@\[\e[36;40m\]\h\[\e[0m\] \[\e[0;31m\]\t\[\e[0m\] \[\e[33;40m\]\w\[\e[0m\]]\[\e[0-31m\]\\\\$ \[\e[0m\]\"
" >> ~/.bash_profile
```

Then exit and reconnect to load the new bash profile.

---

## Phase 3: Download Packages and Copy Files (Parallel)

These two tasks can happen simultaneously - one in Tera Term, one in PowerShell.

### Task A: Download Solaris Packages (via Tera Term)

Connect via Tera Term and run:

```bash
cd /export/home
wget http://download.nust.na/pub3/solaris/intel/5.10/ncurses-5.6-sol10-x86-local.gz
wget http://download.nust.na/pub3/solaris/sunfreeware/pub/unixpackages/x86/5.10/gd-2.0.35-sol10-x86-local.gz
wget http://download.nust.na/pub3/solaris/intel/5.10/gcc-3.4.6-sol10-x86-local.gz
wget http://download.nust.na/pub3/solaris/intel/5.10/openssl-0.9.8-sol10-x86-local.gz
wget http://download.nust.na/pub3/solaris/intel/5.8/db-3.3.11-sol8-intel-local.gz
```

These packages will be available for installation in later phases as needed.

### Task B: Copy TalesWeaver Files (via PowerShell)

While packages are downloading in Tera Term, open PowerShell on Windows and run:

**First, create the destination directory on Solaris:**
```powershell
ssh root@10.0.0.249 "mkdir -p /tw404"
```

**Then copy files:**
```powershell
scp -r "C:\tw404\newpath\tw404" root@10.0.0.249:/
```

(SSH options are already configured in `$env:USERPROFILE\.ssh\config` from Phase 2)

Both tasks can complete independently. Continue to Phase 4 once both are finished.

---

## Phase 4: Install Packages and Create Library Links

After downloads complete and files are copied, connect via Tera Term and run:

```bash
cd /export/home
gunzip *.gz
```

Then install each package separately. For each command below, press **Enter** at the prompt to accept all packages (default is `all`):

```bash
pkgadd -d openssl-0.9.8-sol10-x86-local
```
Press Enter when prompted (select all packages)

```bash
pkgadd -d ncurses-5.6-sol10-x86-local
```
Press Enter when prompted

```bash
pkgadd -d gcc-3.4.6-sol10-x86-local
```
Press Enter when prompted

```bash
pkgadd -d gd-2.0.35-sol10-x86-local
```
Press Enter when prompted

```bash
pkgadd -d db-3.3.11-sol8-intel-local
```
Press Enter when prompted

After all packages are installed, create the required library symlinks. These can all be pasted together:

```bash
ln -s /opt/csw/lib/libncurses.so.5 /usr/local/lib/libncurses.so.5
ln -s /opt/csw/lib/libiconv.so.2.5.0 /usr/lib/libiconv.so.2
ln -s /usr/local/lib/libgcc_s.so.1 /lib/libgcc_s.so.1
ln -s /usr/local/lib/libstdc++.so.6.0.3 /lib/libstdc++.so.6
ln -s /usr/local/BerkeleyDB.3.3/lib/libdb-3.3.so /lib/libdb-3.3.so
```

All symlinks will be created successfully.

Then fix permissions on the TalesWeaver files:

```bash
chmod -R 755 /tw404
```

---

## Phase 5: Remove Pre-installed MySQL Packages (if present)

Check if MySQL packages are installed:

```bash
pkginfo | grep -i mysql
```

If any packages are found (SUNWmysqlr, SUNWmysqlt, SUNWmysqlu), remove them one at a time. For each command below, press **Enter** at the confirmation prompt:

```bash
pkgrm SUNWmysqlr
```
Press Enter when prompted

```bash
pkgrm SUNWmysqlt
```
Press Enter when prompted

```bash
pkgrm SUNWmysqlu
```
Press Enter when prompted

If no MySQL packages are found, you can skip this phase and proceed to Phase 6.

---

## Phase 6: Compile and Install MySQL from Source

Create the build directory:

```bash
mkdir /usr/local
cd /usr/local
```

Download MySQL source directly from Solaris using wget (use HTTP, not HTTPS):

```bash
cd /usr/local
wget http://distro.ibiblio.org/amigolinux/download/Utils/mysql-5.0.51/mysql-5.0.51.tar.gz
```

Extract the archive:

```bash
gunzip -cd mysql-5.0.51.tar.gz | gtar xvpf -
```

Enter the MySQL directory:

```bash
cd mysql-5.0.51
```

Run the configure script (this takes several minutes):

```bash
CC=gcc CFLAGS="-O3 -fomit-frame-pointer-DHAVE_CURSES_H" \
CXX=gcc \
CXXFLAGS="-O3 -fomit-frame-pointer-felide-constructors \
-fno-exceptions -fno-rtti-DHAVE_CURSES_H" \
./configure --prefix=/usr/local/mysql --sysconfdir=/etc --localstatedir=/usr/local/mysql/data --enable-assembler --with-mysqld-ldflags=-all-static --with-charset=utf8 --with-extra-charsets=all
```

After configure completes, compile MySQL (this will take several minutes):

```bash
gmake
```

After gmake completes, install it:

```bash
gmake install
```

Configure MySQL user, group, and permissions:

```bash
mkdir /usr/local/mysql/data
groupadd mysql
useradd -g mysql mysql
chgrp -R mysql /usr/local/mysql
chown -R root /usr/local/mysql
chown -R mysql /usr/local/mysql/data
chmod -R 770 /usr/local/mysql
```

Setup MySQL startup scripts and configuration:

```bash
cp /usr/local/mysql/share/mysql/mysql.server /etc/init.d/
ln /etc/init.d/mysql.server /etc/rc3.d/S99mysql
ln /etc/init.d/mysql.server /etc/rc0.d/K01mysql
cp /usr/local/mysql/share/mysql/my-small.cnf /etc/my.cnf
```

---

## Phase 7: Configure MySQL Character Set for Shift-JIS

Edit `/etc/my.cnf` to add character set configuration. You can use either:

**Option 1: Using vi in Tera Term**

```bash
vi /etc/my.cnf
```

**Option 2: Using WinSCP with Notepad++**

- Download WinSCP from: https://winscp.net/eng/download.php
- Connect to `root@10.0.0.249` with SSH settings from Phase 2
- Navigate to `/etc/my.cnf`
- Right-click → Edit → Choose Notepad++ as default editor

**Edit the file and make these changes:**

Find the section `# [client]` and add at the end of that section:

```
default-character-set=sjis
```

Find the section `# [mysqld]` and add at the end of that section:

```
default-character-set = utf8
skip-character-set-client-handshake
character-set-server = utf8
collation-server = utf8_general_ci
init-connect = SET NAMES utf8
```

Save the file and close the editor.

---

## Phase 8: Initialize MySQL Database

Before starting MySQL, initialize the system databases:

```bash
/usr/local/mysql/bin/mysql_install_db --user=mysql
```

This creates the necessary system tables (mysql, information_schema, etc.)

---

## Phase 9: Start MySQL Server

After initialization completes, start MySQL:

```bash
/usr/local/mysql/bin/mysqld_safe --datadir=/usr/local/mysql/data --user=mysql &
```

Wait a few seconds for it to start, then verify it's running:

```bash
ps aux | grep mysqld
```

You should see the mysqld process running.

---

## Phase 10: Set MySQL Root Password

Once MySQL is confirmed running, set the root password for local access:

```bash
/usr/local/mysql/bin/mysqladmin -u root password 'your-root-password-here'
```

Also secure the hostname-based root access:

```bash
/usr/local/mysql/bin/mysqladmin -u root -h localhost password 'your-root-password-here'
```

Replace `your-root-password-here` with a secure password. Note the password for future database connections.

Test the connection:

```bash
/usr/local/mysql/bin/mysql -u root -p
```

Enter the password you just set. If successful, you'll see the `mysql>` prompt. Exit with:

```
exit
```

---

## Phase 11: Create Game Databases and Tables

Connect to MySQL:

```bash
/usr/local/mysql/bin/mysql -u root -p
```

Enter your root password.

Create the gamedb user for the hostname 'unknown' (the game server reports as 'unknown' when connecting):

```sql
CREATE USER 'gamedb'@'unknown' IDENTIFIED BY 'vlql=nrt';
GRANT ALL PRIVILEGES ON *.* TO 'gamedb'@'unknown' WITH GRANT OPTION;
UPDATE `mysql`.`user` SET `Password`='6e4637a643a8fc2b' WHERE `Host`='unknown' AND `User`='gamedb';
FLUSH PRIVILEGES;
```

**Note:** Do NOT change the values `vlql=nrt`, `6e4637a643a8fc2b`, or `gamedb` - these are required by the TalesWeaver server.

**Important:** The game server reports its hostname as 'unknown' when connecting to MySQL, so the user must be created for the 'unknown' host exactly.

Create game databases:

```sql
create database jtales12_account;
create database jtales12_castle;
create database jtales12_episode;
create database jtales12_friendList;
create database jtales12_gamestat;
create database jtales12_group;
create database jtales12_guild;
create database jtales12_pet;
create database jtales12_refuse;
create database jtales12_share;
```

Now create the tables with proper structure and Shift-JIS encoding. Execute each section separately:

**Account Database:**

```sql
use jtales12_account;

DROP TABLE IF EXISTS account;
DROP TABLE IF EXISTS delete_character_list;
   
CREATE TABLE `account` (
`tid` int(10) unsigned NOT NULL,
 `tusername` varchar(30) NOT NULL,
 `tpassword` varchar(30) NOT NULL,
 `temail` varchar(50) NOT NULL,
 `tregtime` datetime default NULL,
 `tregip` varchar(45) NOT NULL,
 `id` int(10) unsigned NOT NULL auto_increment,
 `username` varchar(30) NOT NULL,
 `password` varchar(30) NOT NULL,
 `email` varchar(50) NOT NULL,
 `regtime` datetime default NULL,
 `regip` varchar(45) NOT NULL,
 `passwd` varchar(30) NOT NULL,
 PRIMARY KEY  USING BTREE (`id`),
 UNIQUE KEY `UNIQUE` USING BTREE (`tusername`)
) ENGINE=InnoDB DEFAULT CHARSET=sjis ROW_FORMAT=DYNAMIC;
   
CREATE TABLE `delete_character_list` (
`requestdate` datetime NOT NULL,
 PRIMARY KEY  USING BTREE (`requestdate`)
) ENGINE=InnoDB DEFAULT CHARSET=sjis;
```

**Castle Database:**

```sql
use jtales12_castle;
   
DROP TABLE IF EXISTS castle;
DROP TABLE IF EXISTS castle_entrusted;
DROP TABLE IF EXISTS guardian;
   
CREATE TABLE `castle` (
`castleNum` int(11) NOT NULL default '0',
 `castleName` varchar(50) default NULL,
 `king` varchar(20) default NULL,
 `guild` varchar(20) default NULL,
 `loser` varchar(20) default NULL,
 `state` int(11) NOT NULL default '0',
 `fortitude` int(11) NOT NULL default '0',
 `remainTime` int(11) NOT NULL default '0',
 `victories` int(11) NOT NULL default '0',
 `challengerGuild` varchar(20) default NULL,
 `victoryTick` int(11) NOT NULL default '0',
 `readyGuild` varchar(20) default NULL,
 UNIQUE KEY `castleNum` (`castleNum`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
   
CREATE TABLE `castle_entrusted` (
 `castleNum` int(11) NOT NULL default '0',
 `image` blob
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
 
CREATE TABLE `guardian` (
 `guardianNumber` int(11) NOT NULL default '0',
 `ownerGuildName` varchar(20) NOT NULL default 'NO_NAME',
 `catchedTick` int(10) unsigned NOT NULL default '0',
 UNIQUE KEY `guardianNumber` (`guardianNumber`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
```

**Episode Database:**

```sql
use jtales12_episode;
 
DROP TABLE IF EXISTS goodwill_data;
DROP TABLE IF EXISTS switch_data;
DROP TABLE IF EXISTS switch_log;
   
CREATE TABLE `goodwill_data` (
`characterid` varchar(32) NOT NULL default '',
 `goodwill` blob NOT NULL,
 PRIMARY KEY  (`characterid`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
   
CREATE TABLE `switch_data` (
`characterid` varchar(32) NOT NULL default '',
 `episode` smallint(5) unsigned NOT NULL default '0',
 `switch` blob NOT NULL,
 UNIQUE KEY `characterId` (`characterid`,`episode`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
 
CREATE TABLE `switch_log` (
`name` varchar(50) NOT NULL default '',
 `episode` int(11) NOT NULL default '0',
 `tick` int(10) unsigned NOT NULL default '0',
 `log` varchar(250) NOT NULL default '',
 UNIQUE KEY `switchLogUniqueIndex` (`name`,`episode`,`tick`,`log`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
```

**FriendList Database:**

```sql
use jtales12_friendList;
 
DROP TABLE IF EXISTS FLfriend;
DROP TABLE IF EXISTS FLgroup;
 
CREATE TABLE `FLfriend` (
`myName` varchar(50) NOT NULL default '',
 `friendName` varchar(50) NOT NULL default '',
 `groupId` int(11) default '0',
 UNIQUE KEY `uIndex` (`myName`,`friendName`),
 KEY `myList` (`myName`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
 
CREATE TABLE `FLgroup` (
`id` int(11) NOT NULL default '0',
 `name` varchar(50) NOT NULL default '',
 `ownerName` varchar(50) NOT NULL default '',
 UNIQUE KEY `idIndex` (`id`,`ownerName`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
```

**GameStat Database:**

```sql
use jtales12_gamestat;

DROP TABLE IF EXISTS GSMonster;
DROP TABLE IF EXISTS GSSoldItem;
DROP TABLE IF EXISTS GSWorld;
   
CREATE TABLE `GSMonster` (
`monName` varchar(50) NOT NULL default '',
 `level` int(11) default '0',
 `killed` int(11) default '0',
 `updatetime` timestamp NOT NULL default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
 
CREATE TABLE `GSSoldItem` (
`itemName` varchar(50) NOT NULL default '',
 `sold` int(11) default '0',
 `updatetime` timestamp NOT NULL default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
 
CREATE TABLE `GSWorld` (
 `worldName` varchar(50) NOT NULL default '',
 `level` int(11) NOT NULL default '0',
 `inCount` int(11) default '0',
 `stayTime` int(11) default '0',
 `updatetime` timestamp NOT NULL default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
```

**Group Database:**

```sql
use jtales12_group;

DROP TABLE IF EXISTS member;
 
CREATE TABLE `member` (
`name` varchar(50) NOT NULL default '',
 `team` varchar(50) NOT NULL default '',
 `type` int(11) NOT NULL default '0',
 `db` int(11) NOT NULL default '0',
 `pk` int(11) NOT NULL default '0',
 `tick` int(11) NOT NULL default '0',
 `level` int(11) NOT NULL default '0',
 `state` bigint(20) unsigned NOT NULL default '0',
 UNIQUE KEY `nameindex` (`name`),
 KEY `teamindex` (`team`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis PACK_KEYS=1;
```

**Guild Database:**

```sql
use jtales12_guild;

DROP TABLE IF EXISTS guild;
DROP TABLE IF EXISTS guildAnnounce;
DROP TABLE IF EXISTS guildBank;
DROP TABLE IF EXISTS guildBankLog;
DROP TABLE IF EXISTS guildLog;
DROP TABLE IF EXISTS guildMember;
   
CREATE TABLE `guild` (
`name` varchar(50) NOT NULL default '',
 `type` int(11) NOT NULL default '0',
 `subType` int(11) NOT NULL default '0',
 `birthTick` int(10) unsigned NOT NULL default '0',
 `markType` int(11) NOT NULL default '0',
 `markResourceId` int(10) unsigned NOT NULL default '0',
 `level` int(11) NOT NULL default '0',
 `acceptMinLevel` int(11) NOT NULL default '0',
 `acceptMaxLevel` int(11) NOT NULL default '0',
 `hpUrl` varchar(80) NOT NULL default '',
 `intro` varchar(250) NOT NULL default '',
 `exp` int(10) unsigned NOT NULL default '0',
 `goodwill` int(10) unsigned NOT NULL default '0',
 `voteTick` int(10) unsigned NOT NULL default '0',
 `taxLevied` int(11) NOT NULL default '0',
 `currentTax` int(11) NOT NULL default '0',
 `taxDelayed` int(11) NOT NULL default '0',
 `collectingTaxTick` int(10) unsigned NOT NULL default '0',
 `taxDelayMonth` int(11) NOT NULL default '0',
 `state` int(10) unsigned NOT NULL default '0',
 UNIQUE KEY `guildNameIndex` (`name`,`type`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
 
CREATE TABLE `guildAnnounce` (
`name` varchar(50) NOT NULL default '',
 `type` int(11) NOT NULL default '0',
 `tick` int(10) unsigned NOT NULL default '0',
 `announce` varchar(250) NOT NULL default '',
 UNIQUE KEY `guildAnnounceUniqueIndex` (`name`,`type`,`tick`,`announce`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
 
CREATE TABLE `guildBank` (
`guildName` varchar(50) NOT NULL default '',
 `guildType` int(11) NOT NULL default '0',
 `attribute` blob NOT NULL,
 UNIQUE KEY `guildBankIndex` (`guildName`,`guildType`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
 
CREATE TABLE `guildBankLog` (
`name` varchar(50) NOT NULL default '',
 `type` int(11) NOT NULL default '0',
 `tick` int(10) unsigned NOT NULL default '0',
 `log` varchar(250) NOT NULL default '',
 UNIQUE KEY `guildBankLogUniqueIndex` (`name`,`type`,`tick`,`log`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
 
CREATE TABLE `guildLog` (
`name` varchar(50) NOT NULL default '',
 `type` int(11) NOT NULL default '0',
 `tick` int(10) unsigned NOT NULL default '0',
 `log` varchar(250) NOT NULL default '',
 UNIQUE KEY `guildLogUniqueIndex` (`name`,`type`,`tick`,`log`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
 
CREATE TABLE `guildMember` (
`name` varchar(50) NOT NULL default '',
 `type` int(11) NOT NULL default '0',
 `guildName` varchar(50) NOT NULL default '',
 `guildType` int(11) NOT NULL default '0',
 `title` varchar(50) NOT NULL default '',
 `DBID` int(11) NOT NULL default '0',
 `level` int(11) NOT NULL default '0',
 `joinTick` int(10) unsigned NOT NULL default '0',
 `resignTick` int(10) unsigned NOT NULL default '0',
 `rank` int(11) NOT NULL default '0',
 `vote` int(11) NOT NULL default '0',
 `logoutTick` int(10) unsigned NOT NULL default '0',
 `exp` int(10) unsigned NOT NULL default '0',
 `state` int(10) unsigned NOT NULL default '0',
 UNIQUE KEY `guildMemberNameIndex` (`guildName`,`guildType`,`name`),
 KEY `guildMemberIndex` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
```

**Pet Database:**

```sql
use jtales12_pet;

DROP TABLE IF EXISTS pet;
   
CREATE TABLE `pet` (
 `owner` varchar(16) NOT NULL default '',
 `type` int(11) NOT NULL default '-1',
 `nutrition` int(11) NOT NULL default '0',
 `nutritionTick` int(11) NOT NULL default '0',
 `sanitation` int(11) NOT NULL default '0',
 `sanitationTick` int(11) NOT NULL default '0',
 `remainTime` int(11) NOT NULL default '0',
 `eggid` varchar(8) NOT NULL default 'NO_NAME',
 `dbid` varchar(8) NOT NULL default 'NO_NAME',
 `color` int(11) NOT NULL default '0',
 `name` varchar(32) NOT NULL default 'Pet',
 `level` int(11) NOT NULL default '0',
 `vital` int(11) NOT NULL default '0',
 `exp` int(11) NOT NULL default '0',
 `birthTime` int(11) NOT NULL default '0',
 `bHibernated` int(11) NOT NULL default '0',
 `partnerOwner` varchar(16) NOT NULL default 'NO_NAME',
 `partner` int(11) NOT NULL default '0',
 `cleanItem` blob NOT NULL,
 `nutritionItem` blob NOT NULL,
 `foodItem` blob NOT NULL,
 `skills` blob,
 UNIQUE KEY `owner` (`owner`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
```

**Refuse Database:**

```sql
use jtales12_refuse;
 
DROP TABLE IF EXISTS refuse;
 
CREATE TABLE `refuse` (
 `ownerName` varchar(50) NOT NULL default '',
 `otherName` varchar(50) NOT NULL default '',
 UNIQUE KEY `uIndex` (`ownerName`,`otherName`),
 KEY `myList` (`ownerName`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
```

**Share Database:**

```sql
use jtales12_share;

DROP TABLE IF EXISTS share;
   
CREATE TABLE `share` (
 `idx` int(11) NOT NULL auto_increment,
 `receiver` varchar(16) NOT NULL default '',
 `sender` varchar(16) NOT NULL default '',
 `time` int(11) NOT NULL default '0',
 `image` blob,
 `seed` int(11) NOT NULL default '0',
 UNIQUE KEY `shareItemIndex` (`idx`,`receiver`,`sender`)
) ENGINE=MyISAM DEFAULT CHARSET=sjis;
```

Exit MySQL:

```sql
quit;
```

---

## Phase 12: Setting up the Game Server

Before starting the servers, we need to configure the IP addresses and system date.

### Step 12.1: Edit Configuration Files

We need to edit these files and change the IP to your server's internal IP (10.0.0.249):

- `/tw404/db/DB.cfg`
- `/tw404/jtales*/table/DBs.jtales`
- `/tw404/jtales*/table/Servers.jtales`

**Using WinSCP with Notepad++:**

1. Download WinSCP from: https://winscp.net/eng/download.php
2. Connect to `root@10.0.0.249`
3. Navigate to each file listed above
4. Right-click → Edit → Choose Notepad++ as default editor
5. Use **Ctrl-H** to open Find & Replace dialogue
6. Find the old IP address and replace with `10.0.0.249`

**IMPORTANT: Server Date Configuration**

The TalesWeaver server kicks you immediately if the server time is current. For now, set the server date to something old.

From Tera Term, run:

```bash
date 0101000003
```

(This sets date to 00:00 Jan 1, 2003. Format is mmddHHMMYY)

---

## Phase 13: Start the Game Servers

You're now ready to start the servers! Open 4 instances of SSH connections in Tera Term (root).

**Window 1: Start the Database Server (Master Account & Character Files)**

```bash
cd /tw404/db
./db -l ./logs/db.log
```

**Window 2: Start the Login Server (jtales0)**

```bash
cd /tw404/jtales0
./jtales -d 12 jtales0
```

**Window 3: Start the First World Server (jtales1)**

```bash
cd /tw404/jtales1
./jtales -d 12 jtales1
```

**Window 4: Start the Second World Server (jtales2)**

```bash
cd /tw404/jtales2
./jtales -d 12 jtales2
```

The game server files support running different regional servers for different responsibilities. By configuring `/tw404/jtales*/table/Servers.jtales`, you can add or remove world servers and designate certain maps for certain servers (defined in `Maps.jtales`). For this guide, we leave it as is.

---

## Phase 14: Setting up the Game Client

Go back to your Windows PC and locate the TalesWeaver client folder. If installed from the official installer, it's typically at:

```
C:\Program Files (x86)\Nexon\Talesweaver
```

### Step 14.1: Windows 10 Compatibility

Right click on `InphaseNXD.EXE` → click **Properties**.

Under the **Compatibility** tab:
- Check "Reduced color mode"
- Select "16-bit (65535) color" in the dropdown list
- Click OK

This emulates the application in 16-bit color mode, which the game client requires to run correctly in windowed mode.

### Step 14.2: Create Game Launch Batch File

Inside the TalesWeaver folder, create a new batch file called `Launch Game.bat`.

Edit it in Notepad:

```batch
start InphaseNXD.EXE /USE_SERVER 12 /ADDR xxxxxxxxxx /PORT 40000
goto end
```

### Step 14.3: Convert Your Server IP to Decimal Format

Replace `xxxxxxxxxx` with your server IP converted to a single number.

**Formula:** If your server IP is 10.0.0.249:

```
{ [ 249 * 256^3 ] + [ 0 * 256^2 ] + [ 0 * 256^1 ] + 10 }
= { [ 249 * 16777216 ] + [ 0 ] + [ 0 ] + 10 }
= 4178206210
```

So the line becomes:

```batch
start InphaseNXD.EXE /USE_SERVER 12 /ADDR 4178206210 /PORT 40000
goto end
```

### Step 14.4: Test Login

Launch the game with your batch file.

When asked for login credentials, use:
- Username: `test`
- Password: `test`

You can create a new character or use the default one provided. (In-game account creation does not work yet.)

If everything works, congratulations! You've successfully logged into your own TalesWeaver server!

---

## Phase 15: Optimize Server Configuration with Symlinks

To avoid duplicating configuration files across jtales instances, create symlinks so you only need to edit jtales0.

### Step 15.1: Backup Original Directories

```bash
mv /tw404/jtales1/quest /tw404/jtales1/quest_bk
mv /tw404/jtales1/quest_data /tw404/jtales1/quest_data_bk
mv /tw404/jtales1/table /tw404/jtales1/table_bk

mv /tw404/jtales2/quest /tw404/jtales2/quest_bk
mv /tw404/jtales2/quest_data /tw404/jtales2/quest_data_bk
mv /tw404/jtales2/table /tw404/jtales2/table_bk
```

### Step 15.2: Create Symlinks to jtales0

```bash
ln -s /tw404/jtales0/quest /tw404/jtales1/quest
ln -s /tw404/jtales0/quest_data /tw404/jtales1/quest_data
ln -s /tw404/jtales0/table /tw404/jtales1/table

ln -s /tw404/jtales0/quest /tw404/jtales2/quest
ln -s /tw404/jtales0/quest_data /tw404/jtales2/quest_data
ln -s /tw404/jtales0/table /tw404/jtales2/table
```

### Step 15.3: Verify Symlinks

```bash
ls -ld /tw404/jtales*/table
ls -ld /tw404/jtales*/quest
ls -ld /tw404/jtales*/quest_data
```

Expected output shows symlinks (→) for jtales1 and jtales2 pointing to jtales0.

---

## Phase 16: Create Server Start and Stop Scripts

Instead of manually starting servers in separate windows each time, create automated scripts.

### Step 16.1: Create Start Script

Create the file:

```bash
touch /tw404/startTWserver.sh
chmod 700 /tw404/startTWserver.sh
```

Edit with WinSCP and Notepad++ (set character encoding to SJIS, line endings to LF):

```bash
vi /tw404/startTWserver.sh
```

Paste the following script:

```bash
#!/usr/bin/bash

#=====================================================
# 起動ログ書き込みfunction
# $1:プロセス名
#=====================================================

Fnc_START_PROC() {
  STARTDATE=`date +"%Y/%m/%d %H:%M:%S"`
  LOG_FILE=/tw404/logs/$1.log
  
  # ログファイル存在チェック
  LOG_FILE_CHECK=`ls $LOG_FILE 2>/dev/null`
  
  # 既にある場合は改行入れる
  if [ "$LOG_FILE_CHECK" != "" ];then
      echo >> $LOG_FILE
      echo >> $LOG_FILE
  fi
  echo "#--------------------------------------------------------------------------------------" >> $LOG_FILE
  echo "#   $STARTDATE [$1] START" >> $LOG_FILE
  echo "#--------------------------------------------------------------------------------------" >> $LOG_FILE
}

# MySQL起動
/etc/init.d/mysql.server start

# logsフォルダ作成
mkdir -p /tw404/logs

# db起動
Fnc_START_PROC db
cd /tw404/db
./db >> /tw404/logs/db.log &

# jtales起動
list="0 1 2"
for n in ${list};do
  Fnc_START_PROC jtales${n}
  cd /tw404/jtales${n}
  ./start  >> /tw404/logs/jtales${n}.log & 
done

# 60秒置きに監視して落ちているプロセスがあれば再起動する
while [ "a" != "b" ];do
  db_check=`ps -ef|grep ./db|grep -v grep`
  if [ "${db_check}" = "" ];then
      Fnc_START_PROC db
      cd /tw404/db
      ./db >> /tw404/logs/db_`date +"%Y%m%d"`.log &
  fi
  
  jtales0_check=`ps -ef|grep "./jtales -d 12 jtales0"|grep -v grep`
  if [ "${jtales0_check}" = "" ];then
      Fnc_START_PROC jtales0
      cd /tw404/jtales0
      ./start  >> /tw404/logs/jtales0.log &
  fi
  
  jtales1_check=`ps -ef|grep "./jtales -d 12 jtales1"|grep -v grep`
  if [ "${jtales1_check}" = "" ];then
      Fnc_START_PROC jtales1
      cd /tw404/jtales1
      ./start  >> /tw404/logs/jtales1.log &
  fi
  
  jtales2_check=`ps -ef|grep "./jtales -d 12 jtales2"|grep -v grep`
  if [ "${jtales2_check}" = "" ];then
      Fnc_START_PROC jtales2
      cd /tw404/jtales2
      ./start  >> /tw404/logs/jtales2.log &
  fi
  sleep 60
done
```

### Step 16.2: Start the Servers

```bash
/tw404/startTWserver.sh
```

The script creates log files in `/tw404/logs/`:
- `/tw404/logs/db.log`
- `/tw404/logs/jtales0.log`
- `/tw404/logs/jtales1.log`
- `/tw404/logs/jtales2.log`

Monitor logs with:

```bash
tail -f /tw404/logs/jtales0.log
```

### Step 16.3: Create Stop Script

Create the file:

```bash
touch /tw404/endTWserver.sh
chmod 700 /tw404/endTWserver.sh
```

Edit with WinSCP and Notepad++ (set character encoding to SJIS, line endings to LF):

```bash
vi /tw404/endTWserver.sh
```

Paste the following script:

```bash
# DBプロセスKILL
DB_PROC_ID=`ps -ef|grep "./db"|grep -v grep|awk '{print $2}'`
[ "$DB_PROC_ID" ] && kill -9 $DB_PROC_ID >/dev/null

# jtales0プロセスKILL
JTALES_PROC_0=`ps -ef|grep "./jtales -d 12 jtales0"|grep -v grep|awk '{print $2}'`
[ "$JTALES_PROC_0" ] && kill -9 $JTALES_PROC_0 >/dev/null

# jtales1プロセスKILL
JTALES_PROC_1=`ps -ef|grep "./jtales -d 12 jtales1"|grep -v grep|awk '{print $2}'`
[ "$JTALES_PROC_1" ] && kill -9 $JTALES_PROC_1 >/dev/null

# jtales2プロセスKILL
JTALES_PROC_2=`ps -ef|grep "./jtales -d 12 jtales2"|grep -v grep|awk '{print $2}'`
[ "$JTALES_PROC_2" ] && kill -9 $JTALES_PROC_2 >/dev/null
```

### Step 16.4: Stop the Servers

```bash
/tw404/endTWserver.sh
```

Alternatively, press **Ctrl+C** in the window running `startTWserver.sh`.

---

## Phase 17: Create Game Accounts

The in-game account creation system doesn't work properly, so accounts must be created via command-line utility.

### Step 17.1: Obtain Account Creation Tools

You need to get these files from a 3.5 server installation (if available):

- `/tw/db/master/create_master`
- `/tw/db/master/README_uh`
- `/tw/db/master/uh`

Use WinSCP to download these files, then upload them to `/tw404/db/master/` on your Solaris server.

### Step 17.2: Set Permissions

```bash
cd /tw404/db/master/
chmod 777 create_master
chmod 777 README_uh
chmod 777 uh
```

### Step 17.3: Create an Account

Use this format:

```bash
./create_master <UserID> <Password> <Email> <BirthDate> <RealName> <CreationDate> <TicketExpiry> <Sun?> <Gender>
```

Example (creating account "tales" with password "tales"):

```bash
./create_master tales tales 1@tales.com 20151119 1 99999 999999 4 5
```

### Step 17.4: Verify Account Creation

```bash
find /tw404/db/master -name tales
```

**Note:** The initial account creation will have an incorrect password hash. This will be fixed in the next steps.

### Step 17.5: Perform Initial Login (Will Fail)

1. Launch the game client on Windows
2. Try to login with the credentials (will fail)
3. Check the db log to find the hash path: `/tw404/db/master/XX/YY/accounts`

The log will show something like: `master/04/ee/<accountname>`

### Step 17.6: Move Account File to Correct Location

The account file needs to be moved to the hash location discovered in the logs:

```bash
mv /tw404/db/master/01/e0/tales /tw404/db/master/04/ee
```

(Replace `04/ee` with the actual path from the logs)

### Step 17.7: Restart Servers and Retry Login

Restart the servers:

```bash
/tw404/endTWserver.sh
/tw404/startTWserver.sh
```

Or press Ctrl+C in the start script window and run it again.

On Windows, restart the game client and try logging in again with your created credentials.

---

## Phase 18: Configure Automatic Startup on Boot

To have MySQL, the database server, and all three game servers start automatically when the system boots, create a master startup script in `/etc/init.d/`.

### Step 18.1: Create Boot Startup Script

Create the file:

```bash
touch /etc/init.d/jtales-server
chmod 755 /etc/init.d/jtales-server
```

Edit with WinSCP and Notepad++ (set character encoding to SJIS, line endings to LF):

```bash
vi /etc/init.d/jtales-server
```

Paste the following script:

```bash
#!/bin/sh
#
# Startup/shutdown script for TalesWeaver Game Server
# chkconfig: 345 99 01
# description: TalesWeaver Game Server (ENDRE + jtales)

### BEGIN INIT INFO
# Provides: jtales-server
# Required-Start: $local_fs $remote_fs $syslog
# Required-Stop: $local_fs $remote_fs $syslog
# Default-Start: 3 5
# Default-Stop: 0 1 2 6
# Short-Description: TalesWeaver Game Server
# Description: Starts and stops the TalesWeaver Game Server
### END INIT INFO

. /etc/rc.d/init.d/functions

JTALES_HOME=/tw404
JTALES_USER=root

#=====================================================
# 起動ログ書き込みfunction
# $1:プロセス名
#=====================================================

Fnc_START_PROC() {
  STARTDATE=`date +"%Y/%m/%d %H:%M:%S"`
  LOG_FILE=$JTALES_HOME/logs/$1.log
  
  # ログファイル存在チェック
  LOG_FILE_CHECK=`ls $LOG_FILE 2>/dev/null`
  
  # 既にある場合は改行入れる
  if [ "$LOG_FILE_CHECK" != "" ];then
      echo >> $LOG_FILE
      echo >> $LOG_FILE
  fi
  echo "#--------------------------------------------------------------------------------------" >> $LOG_FILE
  echo "#   $STARTDATE [$1] START" >> $LOG_FILE
  echo "#--------------------------------------------------------------------------------------" >> $LOG_FILE
}

start() {
  echo "Starting TalesWeaver Game Server..."
  
  # MySQL起動
  echo "Starting MySQL..."
  /etc/init.d/mysql.server start
  sleep 2
  
  # logsフォルダ作成
  mkdir -p $JTALES_HOME/logs
  
  # db起動
  echo "Starting ENDRE Database Server..."
  Fnc_START_PROC db
  cd $JTALES_HOME/db
  ./db >> $JTALES_HOME/logs/db.log &
  sleep 2
  
  # jtales起動
  echo "Starting TalesWeaver Game Servers..."
  list="0 1 2"
  for n in ${list};do
    Fnc_START_PROC jtales${n}
    cd $JTALES_HOME/jtales${n}
    ./start  >> $JTALES_HOME/logs/jtales${n}.log & 
  done
  
  echo "TalesWeaver Game Server started."
  
  # 60秒置きに監視して落ちているプロセスがあれば再起動する
  sleep 60
  while true;do
    db_check=`ps -ef|grep $JTALES_HOME/db/db|grep -v grep`
    if [ -z "${db_check}" ];then
        Fnc_START_PROC db
        cd $JTALES_HOME/db
        ./db >> $JTALES_HOME/logs/db_`date +"%Y%m%d"`.log &
    fi
    
    jtales0_check=`ps -ef|grep "./jtales -d 12 jtales0"|grep -v grep`
    if [ -z "${jtales0_check}" ];then
        Fnc_START_PROC jtales0
        cd $JTALES_HOME/jtales0
        ./start  >> $JTALES_HOME/logs/jtales0.log &
    fi
    
    jtales1_check=`ps -ef|grep "./jtales -d 12 jtales1"|grep -v grep`
    if [ -z "${jtales1_check}" ];then
        Fnc_START_PROC jtales1
        cd $JTALES_HOME/jtales1
        ./start  >> $JTALES_HOME/logs/jtales1.log &
    fi
    
    jtales2_check=`ps -ef|grep "./jtales -d 12 jtales2"|grep -v grep`
    if [ -z "${jtales2_check}" ];then
        Fnc_START_PROC jtales2
        cd $JTALES_HOME/jtales2
        ./start  >> $JTALES_HOME/logs/jtales2.log &
    fi
    sleep 60
  done
}

stop() {
  echo "Stopping TalesWeaver Game Server..."
  
  # DBプロセスKILL
  DB_PROC_ID=`ps -ef|grep "$JTALES_HOME/db/db"|grep -v grep|awk '{print $2}'`
  [ -n "$DB_PROC_ID" ] && kill -9 $DB_PROC_ID >/dev/null 2>&1
  
  # jtales0プロセスKILL
  JTALES_PROC_0=`ps -ef|grep "./jtales -d 12 jtales0"|grep -v grep|awk '{print $2}'`
  [ -n "$JTALES_PROC_0" ] && kill -9 $JTALES_PROC_0 >/dev/null 2>&1
  
  # jtales1プロセスKILL
  JTALES_PROC_1=`ps -ef|grep "./jtales -d 12 jtales1"|grep -v grep|awk '{print $2}'`
  [ -n "$JTALES_PROC_1" ] && kill -9 $JTALES_PROC_1 >/dev/null 2>&1
  
  # jtales2プロセスKILL
  JTALES_PROC_2=`ps -ef|grep "./jtales -d 12 jtales2"|grep -v grep|awk '{print $2}'`
  [ -n "$JTALES_PROC_2" ] && kill -9 $JTALES_PROC_2 >/dev/null 2>&1
  
  # MySQL停止
  /etc/init.d/mysql.server stop
  
  echo "TalesWeaver Game Server stopped."
}

case "$1" in
  start)
    start
    ;;
  stop)
    stop
    ;;
  restart)
    stop
    sleep 2
    start
    ;;
  *)
    echo "Usage: $0 {start|stop|restart}"
    exit 1
esac

exit 0
```

### Step 18.2: Create Boot Symlinks

Create symlinks so the script runs on startup:

```bash
ln -s /etc/init.d/jtales-server /etc/rc3.d/S99jtales-server
ln -s /etc/init.d/jtales-server /etc/rc0.d/K01jtales-server
```

### Step 18.3: Test the Boot Script

Test that the script works correctly:

```bash
/etc/init.d/jtales-server stop
sleep 2
/etc/init.d/jtales-server start
```

Check the log files to verify all services are running:

```bash
tail -f /tw404/logs/db.log
```

### Step 18.4: Verify Boot Startup

The servers will now automatically start when Solaris boots. To verify the configuration:

```bash
ls -la /etc/rc3.d/S99jtales-server
ls -la /etc/rc0.d/K01jtales-server
```

**Note:** The startup order is:
1. MySQL starts first (via existing `/etc/rc3.d/S99mysql`)
2. ENDRE database server starts (`./db`)
3. All three jtales servers start (login, world1, world2)
4. Process monitoring runs every 60 seconds to restart any crashed processes

To manually control the servers after boot:

```bash
# Start all servers
/etc/init.d/jtales-server start

# Stop all servers
/etc/init.d/jtales-server stop

# Restart all servers
/etc/init.d/jtales-server restart
```

---

## Setup Complete

You now have a fully functional TalesWeaver game server running on Solaris 10!

All services (MySQL, ENDRE database, and game servers) will automatically start on system boot and are monitored for crashes with automatic restart capability.
