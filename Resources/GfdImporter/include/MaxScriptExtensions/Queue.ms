
/* 	Queue implementation in MaxScript
	Written by TGE. Please give credit where is due.
*/

struct QueueContainer
(
	m_ItemArray = #(),
	
	fn Count = 
	(
		m_ItemArray.Count
	),
	
	fn IsEmpty =
	(
		m_ItemArray.Count == 0
	),
	
	fn EnqueueItem =
	(
		append ItemArray
	),
	
	fn DequeueItem =
	(
		local result = undefined
		if (not (IsEmpty())) then
		(
			local item = ItemArray[1]
			DeleteItem  m_ItemArray[1]
		)
		
		result
	),
	
	fn Peek =
	(
		local result = undefined
		if (not (IsEmpty())) then
		(
			result = ItemArray[1]
		)
		
		result
	)
)
