%error = ForceRequiredAddOn("Brick_LLSylvanorTrees");

if(%error != $Error::AddOn_NotFound)
{
	exec("./treeGun.cs");
}