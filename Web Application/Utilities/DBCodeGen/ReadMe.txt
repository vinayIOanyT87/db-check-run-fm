This is a project using T4 templates to generate CRUD sql scripts.

Notes:

- You have to generate the scripts twice in the following way:
  - Generate the script.
  - Run against your target database
  - Generate the script again
  - (The reason is the script actually look up the table infomation to generate some of the stored procedures.)


- To use T4 scripts in Visual Studio, you may need to install T4 toolkit and T4 Editor.  
  I believe that you need T4 Editor but you don't need T4 Toolkit if you are using VS 2010.

- Most of the classes are written in a little bit of rush not following the standards 100%. 
  A lot of classes and places could be refactored better 
  and more error checking can be added
  if I have more time...


- How to run T4 script?
  - save the script
  or
  - right click on the script and select Run Custom Tool.


- quick T4 syntax(the ones that I have used):
	- <#@ template #> directive
	  - specify template charteristics, eg. <#@ template language="C#v3.5" #>
	- <#@ output #> directive
	  - specify output file charteristics, e.g. <#@ output extension="sql" #>
	- <#@ assembly #> directive
	  - specify refenenced assembly
	- <#@ import #> directive
	  - equivalent to Using	
	- <#@ include #> directive
	  - include the specify file
	  - be careful, not to invoke multiple times
	- <#+ #> class feature block
	- <#= #> expression block
	- <# #> statement block
	- You create template by subclassing standard T4 template.
	  Then you override the TransformText function which returns the generated string.
	  Typically, you use Text block and use Write/WriteLine to write out to the ouput.
	  Then you call this.GenerationEnvironment.ToString() to return the string.

- one of the tutorials: http://www.olegsych.com/2007/12/text-template-transformation-toolkit/

- A typical script could be like AutoDistributionRule.tt
  - It includes the generator tempalte
  - It includes configuration template
  - It calls the generator and pass the configuration to the generator.
  - The way it works with T4's scripts is when you run a script, it is like running a big long script 
	by using include directive.
  - FMTemplateBase is the base template for all templates.
	It has RenderDrop method and RenderStandard method to generate different types of SQL.  
	RendorDrop generates the Drop SQL and RenderStandard generates the regular CRUD SQL.
	Subclasses override RenderStandardCore and RenderDropCore method to do customizations.
  - AutoDistributionConfig has the configuration information.
	- CreateMainTable method has a sample of defining a simple table
	  - It creates a table using CreateTemplate
	  - It calls AddStandardTemplates to add Insert/Update/Delete/Select templates
	- NewConfig method has a sample of defining an Entity To Site Map
	- NewConfig method also has a sample of defining a simple map.
	- At the end of the NewConfig method, it calls the DeleteApplication template.


I used sample of 1 to create these scripts so they most likely need to be modified to be more generalized.

 