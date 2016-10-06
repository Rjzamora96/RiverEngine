MoveLeft = {
	Update=function(MoveLeft, dt)
		print("Hello!")
		MoveLeft.entity.transform.position.y = 300
		MoveLeft.entity.transform.rotation = MoveLeft.entity.transform.rotation + dt
		if(MoveLeft.entity.transform.position.x <= 800.0) then
			MoveLeft.entity.transform.position.x = 100.0 * dt + MoveLeft.entity.transform.position.x
		end
	end
}