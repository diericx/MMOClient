local file = {}

function file.exists( path )
	local exists = false
	local fileHandle = io.open( path, "r" )
	if fileHandle then
		exists = true
		io.close( fileHandle ) 
	end 
	return exists
end

function file.load( path )
	local data = nil
	local fileHandle = io.open( path, "rb" )
	if fileHandle then
		data = fileHandle:read( "*a" )
		io.close( fileHandle ) 
	end 
	return data or ''
end

function file.save( data, path ) 
	local success = false 
	local fileHandle = io.open( path, "wb" ) 
	if fileHandle and data then 
		fileHandle:write( data ) 
		io.close( fileHandle ) 
		success = true
	end
	return success 
end

return file