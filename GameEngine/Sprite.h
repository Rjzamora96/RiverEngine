#pragma once

#include "Component.h"
#include "Vector2.h"
#include "Rectangle.h"
namespace RiverEngine
{
	class Sprite : public Component
	{
	public:
		typedef void(*AddFunc)(Sprite*, std::string);
		typedef void(*ChangeSprite)(Sprite*, std::string);
		typedef void(*AddTexture)(std::string);
		Sprite(std::string path = "") : m_sprite(path), origin(Vector2()) {}
		static AddFunc addSprite;
		static ChangeSprite changeSprite;
		static AddTexture addTexture;
		std::string GetSprite() const { return m_sprite; }
		void SetSprite(std::string path)
		{
			if (changeSprite != 0) changeSprite(this, path);
			m_sprite = path;
		}
		unsigned int id;
		Vector2 origin;
		bool usesRect = false;
		RiverEngine::Rectangle rect;
	private:
		std::string m_sprite;
	};
}