Component.property("speed", 100.0)

local goingForward = true

function Update(self, dt)
	self.entity.transform.position.y = 300
	self.entity.transform.rotation = self.entity.transform.rotation + dt
	if Input.isKeyDown("Up") then
		self.speed = self.speed + (100.0 * dt)
	elseif Input.isKeyDown("Down") then
		self.speed = self.speed - (100.0 * dt)
	end
	if goingForward then
		if self.entity.transform.position.x <= 800.0 then
			self.entity.transform.position.x = self.speed * dt + self.entity.transform.position.x
		else
			goingForward = false
			self.entity:getComponent("Sprite").image = "dog.png"
		end
	else
		if self.entity.transform.position.x >= 0 then
			self.entity.transform.position.x = -self.speed * dt + self.entity.transform.position.x
		else
			goingForward = true
			self.entity.sprite.image = "cat.png"
		end
	end
end