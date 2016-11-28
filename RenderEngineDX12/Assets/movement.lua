Component.Property("speed", 100)

local lastLastPosition = Vector2(0,0)
local lastPosition = Vector2(0,0)

function Initialize(self)
end

function Update(self, dt)
	lastLastPosition.x = lastPosition.x
	lastLastPosition.y = lastPosition.y
	lastPosition.x = self.entity.transform.position.x
	lastPosition.y = self.entity.transform.position.y
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

function OnMessage(self, id, message, sender)
	if(id == "collision") then
		self.entity.transform.position.x = lastLastPosition.x
		self.entity.transform.position.y = lastLastPosition.y
	end
end