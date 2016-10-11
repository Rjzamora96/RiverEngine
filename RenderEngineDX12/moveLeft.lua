Component.Property("speed", 100)

local goingForward = true

function Update(self, dt)
	self.entity.transform.position.y = 300
	self.entity.transform.rotation = self.entity.transform.rotation + dt
	if goingForward then
		if self.entity.transform.position.x <= 800.0 then
			self.entity.transform.position.x = self.speed * dt + self.entity.transform.position.x
		else
			goingForward = false
		end
	else
		if self.entity.transform.position.x >= 0 then
			self.entity.transform.position.x = -self.speed * dt + self.entity.transform.position.x
		else
			goingForward = true
		end
	end
end