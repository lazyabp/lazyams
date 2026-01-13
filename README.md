# LazyAMS - 轻量级后台权限管理系统

LazyAMS 是一个基于 ASP.NET Core 8 构建的、设计简洁、结构清晰的后台权限管理系统。它采用了Clean Architecture（清晰架构）思想，旨在为中小型企业级应用提供一个稳定、可扩展、易于维护的后端开发脚手fe架。

## ✨ 主要功能

- **用户管理**: 提供用户的增、删、改、查以及密码重置功能。
- **角色与权限**: 支持将权限分配给角色，再将角色分配给用户，实现灵活的权限控制。
- **菜单管理**: 根据用户权限动态生成前端路由和菜单。
- **身份认证**: 基于 JWT (JSON Web Tokens) 的无状态身份验证机制。
- **动态权限验证**: 基于策略的动态权限检查，精准控制每个API端点的访问权限。
- **数据种子**: 内置初始化数据（如管理员账户、角色、菜单等），方便快速启动。
- **缓存**: 集成内存缓存，提升热点数据的访问性能。
- **全局异常处理**: 统一的异常捕获和友好的错误响应格式。
- **日志记录**: 使用 NLog 记录应用程序日志，方便调试和监控。

## 🏛️ 技术架构

项目遵循Clean Architecture原则，将项目分层以实现关注点分离(SoC)。

- **`Lazy.Core`**: 核心基础层。提供整个解决方案的通用构建块，如依赖注入接口 (`IScopedDependency` 等)、自定义异常、授权策略、缓存接口、通用工具类等。它不依赖于任何其他项目层。
- **`Lazy.Model`**: 数据与领域模型层。定义了应用的核心业务实体（Entities）、数据库上下文（DbContext）以及EF Core的数据迁移（Migrations）。
- **`Lazy.Shared`**: 共享层。包含跨层共享的常量、枚举、权限定义等。
- **`Lazy.Application.Contracts`**: 应用契约层。定义应用服务的接口 (`IUserService` 等) 和数据传输对象 (DTOs)，作为表现层与应用层之间的通信契约。
- **`Lazy.Application`**: 应用服务层。实现了应用契约层定义的接口，包含了核心的业务逻辑。它编排领域对象来完成具体的业务操作。
- **`WebApi`**: API表现层。作为应用的入口点，包含了API控制器（Controllers）、中间件（Middlewares）、配置文件（`appsettings.json`）、依赖注入容器的配置和应用的启动逻辑 (`Program.cs`)。它依赖于应用层和核心层。
- **`Lazy.UnitTest`**: 单元测试层。用于对应用服务和核心功能进行单元测试，确保代码质量。

这种分层结构确保了项目的高内聚、低耦合，使得各个部分可以独立开发和测试。

## 🛠️ 技术栈

- **框架**: .NET 8 / ASP.NET Core 8
- **ORM**: Entity Framework Core 8
- **数据库**: 可支持 SQL Server, MySQL, PostgreSQL, SQLite 等 (由EF Core支持)
- **依赖注入**: Autofac
- **日志**: NLog
- **密码加密**: BCrypt.net
- **身份认证**: JWT (JSON Web Tokens)
- **ID生成**: Snowflake 雪花算法

## 🚀 如何开始

### 1. 环境准备

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 或其他代码编辑器
- 一款数据库，例如 SQL Server LocalDB

### 2. 配置

- 打开 `WebApi/appsettings.Development.json` 文件。
- 修改 `ConnectionStrings` 部分的 `DefaultConnection`，指向您的数据库实例。

  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=LazyAMS_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
  }
  ```

### 3. 数据库迁移

- 在包管理器控制台(Package Manager Console)中，或使用命令行工具，将默认项目设置为 `WebApi`。
- 执行以下命令来创建和应用数据库迁移：

  ```shell
  # 切换到WebApi项目目录
  cd WebApi

  # 执行数据库迁移
  dotnet ef database update
  ```
  
  如果还没有迁移文件，可以先创建：
  
  ```shell
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```

### 4. 运行项目

- 在 Visual Studio 中直接按 `F5` 启动项目。
- 或者使用命令行：

  ```shell
  dotnet run --project WebApi/WebApi.csproj
  ```

- 项目启动后，可以访问 `https://localhost:<端口号>/swagger` 来查看和测试API接口。

### 5. 初始数据

项目首次启动时，会自动执行数据种子代码，创建默认的管理员账户和相关权限数据。

- **默认管理员账号**: `admin`
- **默认密码**: `123456`

## 🧩 API 接口文档

本项目集成了 Swagger (OpenAPI) 以提供交互式的API文档。当应用运行时，请访问以下地址：

`https://localhost:<端口号>/swagger`

您可以在此页面上浏览所有API端点、查看请求/响应模型并直接进行接口测试。
