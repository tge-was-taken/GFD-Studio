
/*  MaxScript misc helper functions
	Written by TGE. Please give credit where is due.
*/

-- Converts a number into a hexadecimal string
fn HexStr aNum =
(
	"0x" + (bit.intashex(aNum) as string)
)