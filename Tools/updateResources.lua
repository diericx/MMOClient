local lfs = require "lfs"
local json = require "dkjson"
local file = require "file"

local exclude = {
	meta = true,
	DS_Store = true,
}

local scan 
scan = function (current_dir, filenames)
	filenames = filenames or {}
	if not (lfs.chdir (current_dir)) then return end
	for file in lfs.dir(current_dir) do
		if file ~= '.' and file ~= '..' then
			local ext = ((file:reverse():match('(.-)%.'))or'rid'):reverse()

	   		if ext == 'dir' then
	   			scan(current_dir..'/'..file, filenames)
	   		elseif not exclude[ext] then
	   			filenames[current_dir ..'/'.. file] = {path=current_dir,name=file} 
	   		end
		end
	end
	return filenames
end

local CURRENT_DIR = lfs.currentdir()
local RESOURCE_DIR = CURRENT_DIR .. "/../Assets/Resources"
local RESOURCE_JSON_PATH = CURRENT_DIR .. "/../Assets/Resources/resources.txt"

print("")
print "~ Scanning Resources ~"

local filenames = scan( RESOURCE_DIR )

local output = {}
local keys = {}

for fullpath, fileinfo in pairs( filenames ) do
	
	local name = fileinfo.name
	local _,dot = fileinfo.name:find('(.-)%.')

	if dot then
		name = fileinfo.name:sub(1,dot-1)
	end

	local dirBefore = RESOURCE_DIR
	local dir = fileinfo.path:gsub(RESOURCE_DIR .. "/?","")
	if dir == "" then
		dir = name
	else
		dir = dir .. "/" .. name 
	end

	keys[#keys+1] = name
	output[name] = dir
end
print "% done.\n"

table.sort( keys, function(a,b) return string.lower(a) < string.lower(b) end )
-- print(  json.encode( output, { indent = true, keyorder=keys } ) )

file.save( json.encode( output, { indent = true, keyorder=keys } ), RESOURCE_JSON_PATH )