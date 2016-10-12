#pragma once

namespace RiverEngine
{
	class Entity;

	class Engine
	{
	public:
		Engine();
		~Engine();
		static bool Initialize() { Init(); return true; }
	private:
		static bool Init();
	};
}