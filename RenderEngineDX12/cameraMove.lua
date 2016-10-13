Component.Property("speed", 100)

function Initialize(self)

end

function Update(self, dt)
	if Input.IsKeyDown("W") then
		self.entity.transform.position.y = (-self.speed * dt) + self.entity.transform.position.y
	elseif Input.IsKeyDown("S") then
		self.entity.transform.position.y = (self.speed * dt) + self.entity.transform.position.y
	end
	if Input.IsKeyDown("A") then
		self.entity.transform.position.x = (-self.speed * dt) + self.entity.transform.position.x
	elseif Input.IsKeyDown("D") then
		self.entity.transform.position.x = (self.speed * dt) + self.entity.transform.position.x
	end
end

function OnMessage(self, id, msg, sender)

end
