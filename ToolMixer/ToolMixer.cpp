#using <Autofac.dll>
#include "Globals.cpp"

ToolMixer::!ToolMixer() { }

ToolMixer::ToolMixer() { this->tool = gcnew Tool(); }

bool ToolMixer::Activate() { return this->tool->Activate(); }

bool ToolMixer::Deactivate() { return this->tool->Deactivate(); }

ToolMixer::~ToolMixer() { delete this->tool; this->!ToolMixer(); }