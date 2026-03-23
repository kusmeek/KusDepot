# -------
# Reacts
# -------

cd $env:KusDepotSolution\Reacts

npm outdated --long

npm install -g npm-check-updates --verbose

ncu --loglevel verbose

ncu -u --loglevel verbose

npm install --verbose

# ----------------------
# ReactF (Control-Shift)
# ----------------------

# --------------
# ReactG (Shift)
# --------------

cd $env:KusDepotSolution\ReactG

go get -u -v ./...

go mod tidy -v

# ------------
# ReactJ (Alt)
# ------------

cd $env:KusDepotSolution\ReactJ

mvn versions:display-dependency-updates

mvn versions:display-plugin-updates

mvn versions:use-latest-releases

# --------------------
# ReactN (Control-Alt)
# --------------------

cd $env:KusDepotSolution\ReactN

npm outdated --long

npm install --verbose

# ------------------
# ReactP (Shift-Alt)
# ------------------

cd $env:KusDepotSolution\ReactP

py -m venv .venv

.\.venv\Scripts\Activate.ps1

pip list --outdated --verbose

pip install --upgrade -r requirements.txt --verbose

pip freeze > requirements.txt

# ----------------
# ReactR (Control)
# ----------------

cd $env:KusDepotSolution\ReactR

cargo install cargo-outdated

cargo outdated

cargo update --verbose