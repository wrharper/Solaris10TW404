# Copilot Instructions for Solaris10TW404

## Project Overview
This repository contains setup scripts, configuration, and documentation for running a TalesWeaver private server on Solaris 10, including MySQL, ENDRE database, and multiple game server processes. The project is primarily a system integration and deployment guide, not an application codebase.

## Key Components
- **README.md**: The authoritative, step-by-step guide for all setup, configuration, and operational tasks. All workflows and conventions are documented here.
- **/tw404** (on Solaris): The main directory on the Solaris VM where all server binaries, scripts, and logs reside. Not present in this repo, but referenced throughout the guide.
- **/etc/init.d/jtales-server**: Custom init script for starting/stopping all services (MySQL, ENDRE DB, jtales servers) and monitoring/restarting them if they crash.

## Essential Workflows
- **VM Setup**: Follow the README to create and configure a Solaris 10 VM with specific network and locale settings.
- **File Transfer**: Use `scp` and SSH with legacy algorithms (see README for PowerShell commands and SSH config tweaks).
- **Package Installation**: Download and install Solaris packages using `wget`, `gunzip`, and `pkgadd` as described.
- **MySQL Build**: Compile MySQL from source with custom flags for Solaris compatibility (see configure/gmake commands in README).
- **Server Startup**: Use `/etc/init.d/jtales-server start|stop|restart` to control all services. The script also monitors and restarts crashed processes.
- **Logs**: All logs are written to `/tw404/logs/` on the Solaris VM. Check these for troubleshooting.

## Project-Specific Conventions
- **Character Encoding**: Use Shift-JIS (SJIS) for editing server scripts and config files. Set Notepad++ to SJIS and LF line endings when editing via WinSCP.
- **SSH Algorithms**: Legacy algorithms (`diffie-hellman-group1-sha1`, `ssh-rsa`, `hmac-md5`, `hmac-sha1`) are required for Solaris 10 compatibility.
- **Symlinks**: Manual symlink creation is required for some libraries after package install (see README).
- **Startup Order**: MySQL → ENDRE DB → jtales servers. The init script enforces this order and restarts any crashed process every 60 seconds.

## Integration Points
- **External Packages**: All dependencies are installed manually on Solaris (not via package manager). URLs and commands are in the README.
- **Windows Tools**: PowerShell, Tera Term, WinSCP, and Notepad++ are used for remote management and editing.

## Examples
- To start all services: `/etc/init.d/jtales-server start`
- To copy files: `scp -r "C:\tw404\newpath\tw404" root@10.0.0.249:/`
- To edit configs: Use WinSCP + Notepad++ (SJIS, LF)

## References
- See [README.md](../README.md) for all detailed instructions, commands, and troubleshooting steps.
- The `.github/copilot-instructions.md` should be updated if project structure or workflows change.
