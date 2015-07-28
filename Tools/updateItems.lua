local http = require "socket.http"
-- local json = require "dkjson"
-- local file = require "file"
-- local csv  = require "csv"

print "~ Updating items.txt ~"
print "Downloading file"
-- strings, code, header = http.request("http://www.yazarmediagroup.com/tools/zac/zacsItems.php")

-- local function convert(value)
-- 	if string.upper(value) == "TRUE" then
-- 		return true
-- 	elseif string.upper(value) == "FALSE" then
-- 		return false
-- 	elseif tonumber(value) ~= nil then
-- 		return tonumber(value)
-- 	else
-- 		return value
-- 	end
-- end

-- if strings then
-- 	print "Parsing CSV"
	
-- 	local rows = csv.parseRows(strings)
-- 	local output = {}
-- 	local currentName
-- 	local current

-- 	for i=1,#rows do
-- 		rows[i] = csv.parseRow(rows[i])
-- 		local key, value = rows[i][1], rows[i][2]
-- 		if key == "item" then
-- 			if current then 
-- 				output[currentName] = current
-- 			end
-- 			currentName = value
-- 			current = {}
-- 		elseif current and key ~= "" then
-- 			current[key] = convert(value)
-- 		end
-- 	end

-- 	if current then 
-- 		output[currentName] = current
-- 	end

-- 	local sorted = {}
-- 	for key, value in pairs(output) do
-- 		table.insert(sorted, key)
-- 	end

-- 	table.sort(sorted, function(A,B) return A < B end )

-- 	file.save(json.encode(output,{indent=true, keyorder=sorted}),'../Assets/Resources/items.txt')
-- 	print "% done."

-- else
-- 	print ("Failed to fetch items.")
-- end