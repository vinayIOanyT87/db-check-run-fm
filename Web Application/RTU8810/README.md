# Requirements
- Install nodejs (this installation includes npm). It update the environment variables so you need to reboot after the install to use the CLI interface. ( at a prompt run "node -v" to see if its installed).
- Install the Angular CLI interface "npm install -g @angular/cli"

Its much easier to work with VSCode than Visual Studio.  Things to be aware of:
- You need to manually add the new files generated to the project in VS 2017 since VSCode does not work with projects.
- If you use code scaffolding (see below) to generate code, you will need to add src/app.modules.ts since ng generate will write to it.
- Do not check in into TFS the dist, obj and node_modules folders.

After initial deployment of the application from TFS you need to install the npm packages used by the application.
- From a command prompt change to the folder containing the application ( same folder as this file) and run "npm install"
- From Visual Studio, open the RTU8810 Typescript project, right click on "NPM" reference and click on the "Install npm missing Packages".


To work with IIS for testing.
Prerequisite: Make sure that you have installed the package (Development IIS support) in Visual Studio 2017. This is performed through the Visual Studio Installer, under the "ASP.Net and Web Development" section. Also install .NET Core 2.1 hosting bundle.
- Open IIS Manager
- Build RTUWebAPI project, run in debug RTUWebAPI with IIS Express, a folder is created under bin with the following, this folder works in conjunction with the application pool to start VSIISExLauncher and dotnet.exe to host RTUWebApi this will create an application in IIS.
      Project properties > Build tab > Advanced > Set Debugging information to Full
- Build the RTU8810 Angular application (ng build)
    In IIS Manager add an application called "RTU" that points to the the "dist" folder of the RTU8810 app. make sure that the application Pool for the "Default App Pool" has access to the dist folder, the DefaultAppPool will work fine.

# Rtu8810

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 7.1.1.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).
