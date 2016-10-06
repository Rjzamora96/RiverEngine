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
		static Entity* testEntity;
	private:
		static bool Init();
	};
}