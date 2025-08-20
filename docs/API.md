# API Reference

Base URL: `http://localhost:{port}/api`

Auth: Bearer JWT required
- Products: `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]`
- Users: `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]`

Permissions
- Admin and Manager can perform all product operations (create, update, delete, view).
- Only Admin can access Users endpoints.

Headers
- Authorization: `Bearer <token>`
- For JSON: `Content-Type: application/json`
- For upload: `Content-Type: multipart/form-data`

## Products
Route prefix: `/api/products`

Notes
- Listings use client-side pagination and search. Server does not accept pagination/filter query parameters in this version.

### GET /api/products
- Response
  - 200 OK: `ProductDto[]`

### GET /api/products/{id}
- Params: `id: int`
- Responses
  - 200 OK: `ProductDto`
  - 404 NotFound

### POST /api/products
- Content-Type: `multipart/form-data`
- Body (form fields)
  - `Name` (string, required)
  - `Category` (string, required)
  - `Price` (number, >= 0)
  - `StockQuantity` (int)
  - `Description` (string, optional)
  - `ImageUrl` (string, optional; ignored if `imageFile` provided)
  - `imageFile` (file, optional)
- Responses
  - 201 Created: `ProductDto` with `Location` header to `GET /api/products/{id}`
  - 400 BadRequest: model validation errors
  - 401/403 Unauthorized/Forbidden

Example (PowerShell):
```powershell
$token = "<JWT>"
Invoke-RestMethod -Method Post -Uri http://localhost:5000/api/products -Headers @{Authorization="Bearer $token"} -Form @{
  Name='Book'; Category='Media'; Price='19.99'; StockQuantity='20'; Description='A nice book'
  imageFile=Get-Item .\cover.jpg
}
```

### PUT /api/products/{id}
- Content-Type: `multipart/form-data`
- Params: `id: int`
- Body (form fields)
  - All `ProductDto` fields (Id must match route id)
  - `imageFile` (file, optional) — if sent, replaces existing image
  - `removeImage` (bool, optional) — if `true`, removes existing image
- Behavior
  - If `removeImage == true`: existing image file is deleted; `ImageUrl` set null.
  - Else if `imageFile` present: existing file deleted, new saved.
  - Else: keep existing `ImageUrl`.
- Responses
  - 204 NoContent
  - 400 BadRequest (mismatched id or validation)
  - 401/403 Unauthorized/Forbidden
  - 404 NotFound

Example (cURL):
```bash
curl -X PUT "http://localhost:5000/api/products/5" \
  -H "Authorization: Bearer <JWT>" \
  -H "Content-Type: multipart/form-data" \
  -F Id=5 -F Name="Book 2" -F Category="Media" -F Price=21.00 -F StockQuantity=15 -F Description="Updated" \
  -F imageFile=@cover2.jpg
```

### DELETE /api/products/{id}
- Params: `id: int`
- Responses
  - 204 NoContent (deletes product; also deletes image if present)
  - 401/403 Unauthorized/Forbidden

### POST /api/products/{id}/remove-image
- Params: `id: int`
- Body: none
- Responses
  - 204 NoContent (image removed; `ImageUrl` set null)
  - 401/403 Unauthorized/Forbidden
  - 404 NotFound

### ProductDto schema
```json
{
  "Id": 0,
  "Name": "string",         // required, max 200
  "Category": "string",     // required, max 100
  "Price": 0,
  "StockQuantity": 0,
  "Description": "string|null", // max 2000
  "ImageUrl": "string|null"     // set by server when image saved
}
```

## Users (Admin only)
Route prefix: `/api/users`

### GET /api/users
- Response
  - 200 OK: `UserDto[]`

### GET /api/users/{id}
- Params: `id: string`
- Responses
  - 200 OK: `UserDto`
  - 404 NotFound

### POST /api/users
- Content-Type: `application/json`
- Body: `UserDto`
  - For create: include `Email`, `UserName`, `Password`, `Role`, `PhoneNumber?`
- Responses
  - 201 Created: `UserDto` with `Location` header
  - 400 BadRequest: validation errors
  - 401/403 Unauthorized/Forbidden

Example (cURL):
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Authorization: Bearer <JWT>" -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "userName": "user1",
    "password": "P@ssw0rd!",
    "role": "Admin",
    "phoneNumber": "+11234567890"
  }'
```

### PUT /api/users/{id}
- Content-Type: `application/json`
- Params: `id: string` (must match body `Id`)
- Body: `UserDto` (typically exclude `Password` unless your service supports password update)
- Responses
  - 204 NoContent
  - 400 BadRequest (mismatched id or validation)
  - 401/403 Unauthorized/Forbidden

### DELETE /api/users/{id}
- Params: `id: string`
- Responses
  - 204 NoContent
  - 401/403 Unauthorized/Forbidden

### UserDto schema
```json
{
  "Id": "string",
  "Email": "string",        // required, email
  "PhoneNumber": "string|null",
  "UserName": "string",     // required
  "Password": "string|null",// required on create, min length 6
  "Role": "string"          // required, e.g., Admin/Manager
}
```

## Error shape
Validation errors return default ASP.NET Core ProblemDetails or ModelState error format. Example 400 body:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["The Name field is required."]
  }
}
```
