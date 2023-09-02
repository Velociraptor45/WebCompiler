#!/bin/bash

#:: IMPORTANT!! npm 3.x is required to avoid long path exceptions

node_folder=${0%/*}/../src/WebCompiler/Node

if [ -f $node_folder/node_modules.7z ]; then
    echo "node_modules.7z already exists. Nothing to do..."
    exit 0
fi

mkdir -p $node_folder

pushd $node_folder

#        node-sass \ // TODO: GH: is this really needed? it fails to install on OS X
echo Installing packages...
npm install --quiet \
        babel@5.8.34 \
        iced-coffee-script \
        less \
        less-plugin-autoprefix \
        less-plugin-csscomb \
        sass \
        postcss@latest \
        postcss-cli \
        autoprefixer \
        stylus \
        handlebars \
        > /dev/null
npm install --quiet > /dev/null

#if not exist "node_modules/node-sass/vendor/win32-ia32-48" (
#    echo Copying node binding...
#    md "node_modules/node-sass/vendor/win32-ia32-48"
#    copy binding.node "node_modules/node-sass/vendor/win32-ia32-48"
#)

echo Deleting unneeded files and folders...
rm -rf *.html > /dev/null
rm -rf *.markdown > /dev/null
rm -rf *.md > /dev/null
rm -rf *.npmignore > /dev/null
rm -rf *.patch > /dev/null
rm -rf *.txt > /dev/null
rm -rf *.yml > /dev/null
rm -rf .editorconfig > /dev/null
rm -rf .eslintrc > /dev/null
rm -rf .gitattributes > /dev/null
rm -rf .jscsrc > /dev/null
rm -rf .jshintrc > /dev/null
rm -rf CHANGELOG > /dev/null
rm -rf CNAME > /dev/null
rm -rf example.js > /dev/null
rm -rf generate-* > /dev/null
rm -rf gruntfile.js > /dev/null
rm -rf gulpfile.* > /dev/null
rm -rf makefile.* > /dev/null
rm -rf README > /dev/null

rm -rf benchmark bench doc docs example examples images man media scripts test tests testing tst > /dev/null

echo Compressing artifacts and cleans up...
7z a -r -mx9 node_modules.7z node_modules > /dev/null
rm -rf node_modules > /dev/null
rm package.json > /dev/null

#:done
echo Done
popd
