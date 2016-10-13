Component.Property("speed", 100.0)

local goingForward = true

function Initialize(self)
 	self.entity.transform.position.y = 300.0
	self.speed = 0
end

function Update(self, dt)
	self.entity.transform.rotation = self.entity.transform.rotation + dt
	--self.entity.transform.position.y = 300.0
	if Input.IsKeyReleased("Left") then
		self.entity.sprite.image = "dog.png"
	elseif Input.IsKeyPressed("Right") then
		self.entity.sprite.image = "cat.png"
	end
	if Input.IsKeyDown("Up") then
		self.speed = self.speed + (100.0 * dt)
	elseif Input.IsKeyDown("Down") then
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
	if Input.IsKeyDown("Space") then
		message = { amount = self.speed * dt }
		self.entity:sendMessage("MoveUp", message, self)
	end
end

function OnMessage(self, id, msg, sender)
	if id == "MoveUp" then
		self.entity.transform.position.y = self.entity.transform.position.y - msg.amount
		sender.entity:getComponent("Sprite").image = "dog.png"
	end
end