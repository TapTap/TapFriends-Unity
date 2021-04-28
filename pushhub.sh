#!/bin/sh
# 获取当前分支
currentBranch=$(git symbolic-ref --short -q HEAD)

git branch -D main

# shellcheck disable=SC2039
var=("tapcommon-upm" "tapdb-upm" "tapmoment-upm" "tapbootstrap-upm" "taplogin-upm")
# shellcheck disable=SC2039
module=("Common" "TapDB" "Moment" "Bootstrap" "Login")

githubRepoName=("TapCommon" "TapDB" "TapMoment" "TapBootstrap" "TapLogin")

tag=$1

isTapTapRepo=true

function pushGithub(){  
  
  git tag -d $(git tag)
  
  git branch -D main
  
  git subtree split --prefix=Assets/TapTap/$1 --branch main
  
  git remote rm $2
  
  if [ $5 == true ]; then
      echo "Start Push $2 to git@github.com:TapTap/$4-Unity.git"
      git remote add $2 git@github.com:TapTap/$4-Unity.git 
  else
      echo "Start Push $2 to git@github.com:EingShaw/$4.git"
      git remote add $2 git@github.com:EingShaw/$4.git
  fi;
  
  git checkout main --force
  
  git tag $3
  
  git fetch --unshallow main
  
  git push $2 main --force --tags
  
  git checkout $currentBranch --force
}

for ((i=0;i<${#var[@]};i++));do
    pushGithub ${module[$i]} ${var[$i]} $tag ${githubRepoName[$i]} $isTapTapRepo
done   
