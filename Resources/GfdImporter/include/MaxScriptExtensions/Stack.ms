
/* 	Stack implementation in MaxScript
	Written by TGE. Please give credit where is due.
*/

struct StackContainer
(
	m_ItemArray = #(),
	
	fn PeekItem =
	(
		if (m_ItemArray.Count == 0) then
		(
			return undefined
		)
		
		m_ItemArray[m_ItemArray.Count]
	),
	
	fn PopItem = 
	(
		if (m_ItemArray.Count == 0) then
		(
			return undefined
		)
		
		local item = m_ItemArray[m_ItemArray.Count]
		DeleteItem m_ItemArray m_ItemArray.Count
		
		item
	),
	
	fn PushItem item = 
	(
		append m_ItemArray item
	),
	
	fn IsEmpty = 
	(
		m_ItemArray.Count == 0
	),
	
	fn Count =
	(
		m_ItemArray.Count
	)
)