#!/bin/bash -e

appplicationCount=`sfctl application list | grep PackSFApplicationApplication | wc -l`
applicationTypeVersion="1.0.0"
if [[ "$#" != "0" && "$1" != "$applicationTypeVersion" ]];then 	
	applicationTypeVersion="$1"
fi
echo "Deploying with ApplicationTypeVersion $applicationTypeVersion"
if [[ "$appplicationCount" -eq "0" ]];then
    /bin/bash Scripts/install.sh $applicationTypeVersion
    if [[ "$?" -eq "0" ]]; then
    	echo "Successfully deployed application."
	else
		echo "Error occurred while deploying application."
		exit 1 
	fi 
else
    applicationUpgradeCheck= `sfctl application manifest --application-type-name PackSFApplicationApplicationType --application-type-version $applicationTypeVersion &>/dev/null`
    if [[ "$?" -eq  "0" ]]; then
        echo "Application already exists. CleanUp: Undeploying existing app"
        /bin/bash Scripts/uninstall.sh $applicationTypeVersion
        if [[ "$?" -eq "0" ]]; then
            echo "Successfully uninstalled application. Now re-deploying."
        else
            echo "Error occurred while uninstalling application."
            exit 1
        fi
        /bin/bash Scripts/install.sh $applicationTypeVersion
        if [[ "$?" -eq "0" ]]; then
            echo "Successfully deployed application."
        else
            echo "Error occurred while deploying application."
            exit 1
        fi
    else
        echo "Upgrading the Application"
        /bin/bash Scripts/upgrade.sh $applicationTypeVersion
        if [[ "$?" -eq "0" ]]; then
            echo "Successfully upgraded application."
        else
            echo "Error occurred while upgrading application."
            exit 1
        fi
    fi
fi