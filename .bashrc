alias sc='cd C:\\Work\\scts\\Scts-Icms'
alias st='git status'

# Changes branch to develop, pulls the latest, the changes
# back to your current branch and merges it in.
MergeDevelop()
{
	branch=$(git branch | sed -n -e 's/^\* \(.*\)/\1/p')
	git checkout Develop
	git pull origin Develop
	git checkout $branch
	git merge Develop
}

# Pushes the current branch to the same name on origin.
PushCurrent()
{
	branch=$(git branch | sed -n -e 's/^\* \(.*\)/\1/p')
	upperCaseBranch="$(tr '[:lower:]' '[:upper:]' <<< ${branch:0:1})${branch:1}"
	git push origin $branch:$upperCaseBranch
}

# Adds all files to be commited, without the web.config which
# is changed to allow access to the citizen website.
AddWithoutConf()
{
	git add .
	git reset -- Source/Scts.Icms.Web.Service/Web.config
}

# Creates a new branch with the name specified in the parameter.
# Usage: NewBranch <newBranchName>
NewBranch()
{
	git checkout Develop
	git checkout -b $1
}

# Aliases to implement all the above functions.
alias pc='PushCurrent'
alias md='MergeDevelop'
alias addwithoutconf='AddWithoutConf'
alias newbranch='NewBranch'

# Alias to allow the use of TheFuck.
eval "$(thefuck --alias fuck)"

# Alias to show pretty git log
alias log='git log --pretty=oneline'
