#pragma once

#include "Component.h"
#include "Vector2.h"
namespace RiverEngine
{
	class Sprite : public Component
	{
	public:
		typedef void(*AddFunc)(Sprite*, std::string);
		typedef void(*ChangeSprite)(Sprite*, std::string);
		Sprite() : m_sprite(""), origin(Vector2()) {}
		static AddFunc addSprite;
		static ChangeSprite changeSprite;
		std::string GetSprite() { return m_sprite; }
		void SetSprite(std::string path) { if (changeSprite != 0) changeSprite(this, path); m_sprite = path; }
		unsigned int id;
		Vector2 origin;
	private:
		std::string m_sprite;
	};
	typedef void(*AddFunc)(Sprite*, std::string);
	typedef void(*ChangeSprite)(Sprite*, std::string);
	AddFunc Sprite::addSprite;
	ChangeSprite Sprite::changeSprite;
}