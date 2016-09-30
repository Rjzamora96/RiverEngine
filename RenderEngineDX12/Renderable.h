#pragma once
class Renderable
{
public:
	Renderable();
	~Renderable();
	DirectX::SimpleMath::Vector2 m_origin;
	DirectX::SimpleMath::Vector2 m_position;
	float m_layer;
	unsigned m_id;
};

