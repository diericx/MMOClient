local lpeg = require "lpeg"
local P = lpeg.P
local C = lpeg.C
local Ct = lpeg.Ct
local S = lpeg.S

-- Separate file into lines --
local EOF = -P(1)
local endl = S'\n'
local not_endl = (1-endl)
local lines = Ct( (C(not_endl^1) * (endl + EOF))^0 )

-- Separate lines into fields --
local separator = S',\n\r'
local entry = C( (1-separator)^0 ) * separator
local quote = P'"'
local quoted_entry = quote * C( (1-quote)^0 ) * quote * separator
local csv = Ct( (quoted_entry + entry)^0 )

return {
	parseRows = function(string)
		return lpeg.match(lines, string)
	end,
	parseRow = function(string)
		return lpeg.match(csv,string..',')
	end,
}