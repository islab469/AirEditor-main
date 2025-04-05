#!/bin/bash

# ===============================================
# Unity Git Project Setup Tool: unity_git_safety_setup.sh
# Features:
# 1. Add .gitkeep to empty folders 
# 2. Create or update .gitattributes 
# 3. Create or update .gitignore 
# 4. Detect orphan .meta files with no folders 
# ===============================================

echo "Adding .gitkeep to all empty folders..."
find . -type d -empty -not -path "*/.git/*" -exec touch {}/.gitkeep \;

echo "Creating or updating .gitattributes..."
cat > .gitattributes <<EOF
# Force LF line endings
*.cs text eol=lf
*.shader text eol=lf
*.json text eol=lf
*.meta text eol=lf
*.unity text eol=lf
*.asset text eol=lf

# Git LFS for large binary files
*.fbx filter=lfs diff=lfs merge=lfs -text
*.blend filter=lfs diff=lfs merge=lfs -text
*.png filter=lfs diff=lfs merge=lfs -text
*.jpg filter=lfs diff=lfs merge=lfs -text
*.mp4 filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
EOF

echo "Creating or updating .gitignore..."
cat > .gitignore <<EOF
# ========== Unity project ==========

[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
MemoryCaptures/
UserSettings/

# ========== IDE & Compiler junk ==========
.vs/
.idea/
*.csproj
*.sln
*.user
*.pidb
*.pdb
*.tmp
*.suo

# ========== System files ==========
.DS_Store
Thumbs.db
desktop.ini

# ========== Packages & large binaries ==========
*.apk
*.aab
*.unitypackage
*.zip
*.tgz

# ========== Keep .meta files tracked ==========
!Assets/**/*.meta
!Packages/**/*.meta
!ProjectSettings/**/*.meta
!*/.gitkeep
EOF

echo "Checking for orphan .meta files (missing folders)..."
find . -name "*.meta" | while read meta; do
  folder="${meta%.meta}"
  if [[ ! -e "$folder" ]]; then
    echo "Orphan .meta file: $meta (missing asset)"
  fi
done

echo "Done! Your Unity project is now Git-safe and ready!"