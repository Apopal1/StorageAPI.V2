# Storage Management API Documentation

This document provides a detailed description of the Storage Management API endpoints.

## Authentication

### `POST /api/Auth/register`

Registers a new user. The first user registered will be an administrator.

**Request Body:**

*   `firstName` (string, required): The user's first name.
*   `lastName` (string, required): The user's last name.
*   `email` (string, required): The user's email address.
*   `username` (string, required): The user's username.
*   `password` (string, required): The user's password.

**Responses:**

*   `200 OK`: User registered successfully.
*   `400 Bad Request`: If the username or email already exists.

### `POST /api/Auth/login`

Logs in a user.

**Request Body:**

*   `username` (string, required): The user's username.
*   `password` (string, required): The user's password.

**Responses:**

*   `200 OK`: Login successful. Returns a message, user ID, and a boolean indicating if the user is an admin.
*   `401 Unauthorized`: Invalid username or password.

### `POST /api/Auth/webtoken`

Gets a JWT token for a user.

**Request Body:**

*   `username` (string, required): The user's username.
*   `password` (string, required): The user's password.

**Responses:**

*   `200 OK`: Returns a JWT token.
*   `401 Unauthorized`: Invalid username or password.

---

## Storage Items

**Authentication:** Requires JWT Token. Admin role required for `POST`, `PUT`, `DELETE`.

### `GET /api/StorageItems`

Gets all storage items.

**Responses:**

*   `200 OK`: Returns a list of all storage items.

### `GET /api/StorageItems/{id}`

Gets a storage item by its ID.

**Parameters:**

*   `id` (integer, required): The ID of the storage item.

**Responses:**

*   `200 OK`: Returns the storage item.
*   `404 Not Found`: If the storage item is not found.

### `GET /api/StorageItems/by-supplier/{supplierId}`

Gets all storage items from a specific supplier.

**Parameters:**

*   `supplierId` (integer, required): The ID of the supplier.

**Responses:**

*   `200 OK`: Returns a list of storage items.

### `POST /api/StorageItems`

Creates a new storage item.

**Request Body:**

*   (StorageItem object): The storage item to create.

**Responses:**

*   `201 Created`: Returns the created storage item.
*   `400 Bad Request`: If there is an error creating the item.

### `PUT /api/StorageItems/{id}`

Updates a storage item.

**Parameters:**

*   `id` (integer, required): The ID of the storage item to update.

**Request Body:**

*   (StorageItem object): The updated storage item.

**Responses:**

*   `200 OK`: Returns the updated storage item.
*   `404 Not Found`: If the storage item is not found.
*   `400 Bad Request`: If there is an error updating the item.

### `DELETE /api/StorageItems/{id}`

Deletes a storage item.

**Parameters:**

*   `id` (integer, required): The ID of the storage item to delete.

**Responses:**

*   `204 No Content`: If the item is deleted successfully.
*   `404 Not Found`: If the storage item is not found.

---

## Suppliers

**Authentication:** Requires JWT Token. Admin role required for `POST`, `PUT`, `DELETE`.

### `GET /api/Suppliers`

Gets all suppliers.

**Responses:**

*   `200 OK`: Returns a list of all suppliers.

### `GET /api/Suppliers/{id}`

Gets a supplier by its ID.

**Parameters:**

*   `id` (integer, required): The ID of the supplier.

**Responses:**

*   `200 OK`: Returns the supplier.
*   `404 Not Found`: If the supplier is not found.

### `POST /api/Suppliers`

Creates a new supplier.

**Request Body:**

*   (Supplier object): The supplier to create.

**Responses:**

*   `201 Created`: Returns the created supplier.
*   `400 Bad Request`: If there is an error creating the supplier.

### `PUT /api/Suppliers/{id}`

Updates a supplier.

**Parameters:**

*   `id` (integer, required): The ID of the supplier to update.

**Request Body:**

*   (Supplier object): The updated supplier.

**Responses:**

*   `200 OK`: Returns the updated supplier.
*   `404 Not Found`: If the supplier is not found.
*   `400 Bad Request`: If there is an error updating the supplier.

### `DELETE /api/Suppliers/{id}`

Deletes a supplier.

**Parameters:**

*   `id` (integer, required): The ID of the supplier to delete.

**Responses:**

*   `204 No Content`: If the supplier is deleted successfully.
*   `404 Not Found`: If the supplier is not found.
*   `400 Bad Request`: If the supplier is associated with storage items.

---

## Superitems

**Authentication:** Requires JWT Token. Admin role required for `POST`, `PUT`, `DELETE` and for adding/removing sub-items.

### `GET /api/Superitems`

Gets all superitems.

**Responses:**

*   `200 OK`: Returns a list of all superitems.

### `GET /api/Superitems/{id}`

Gets a superitem by its ID.

**Parameters:**

*   `id` (integer, required): The ID of the superitem.

**Responses:**

*   `200 OK`: Returns the superitem.
*   `404 Not Found`: If the superitem is not found.

### `POST /api/Superitems`

Creates a new superitem.

**Request Body:**

*   (SuperitemDto object): The superitem to create, including sub-items.

**Responses:**

*   `201 Created`: Returns the created superitem.

### `PUT /api/Superitems/{id}`

Updates a superitem.

**Parameters:**

*   `id` (integer, required): The ID of the superitem to update.

**Request Body:**

*   (SuperitemDto object): The updated superitem.

**Responses:**

*   `204 No Content`: If the superitem is updated successfully.

### `DELETE /api/Superitems/{id}`

Deletes a superitem.

**Parameters:**

*   `id` (integer, required): The ID of the superitem to delete.

**Responses:**

*   `204 No Content`: If the superitem is deleted successfully.

### `POST /api/Superitems/{id}/subitems/{storageItemId}/{quantity}`

Adds a sub-item to a superitem.

**Parameters:**

*   `id` (integer, required): The ID of the superitem.
*   `storageItemId` (integer, required): The ID of the storage item to add as a sub-item.
*   `quantity` (integer, required): The quantity of the sub-item.

**Responses:**

*   `204 No Content`: If the sub-item is added successfully.

### `DELETE /api/Superitems/{id}/subitems/{storageItemId}`

Removes a sub-item from a superitem.

**Parameters:**

*   `id` (integer, required): The ID of the superitem.
*   `storageItemId` (integer, required): The ID of the storage item to remove.

**Responses:**

*   `204 No Content`: If the sub-item is removed successfully.

### `GET /api/Superitems/{id}/subitems`

Gets all sub-items for a superitem.

**Parameters:**

*   `id` (integer, required): The ID of the superitem.

**Responses:**

*   `200 OK`: Returns a list of sub-items.

### `GET /api/Superitems/{id}/possible`

Calculates how many superitems can be created based on the available quantity of sub-items.

**Parameters:**

*   `id` (integer, required): The ID of the superitem.

**Responses:**

*   `200 OK`: Returns the number of possible superitems.

---

## Outgoing Orders

### `GET /api/OutgoingOrders`

Gets all outgoing orders.

**Responses:**

*   `200 OK`: Returns a list of all outgoing orders.

### `GET /api/OutgoingOrders/{id}`

Gets an outgoing order by its ID.

**Parameters:**

*   `id` (integer, required): The ID of the outgoing order.

**Responses:**

*   `200 OK`: Returns the outgoing order.
*   `404 Not Found`: If the outgoing order is not found.

### `POST /api/OutgoingOrders`

Creates a new outgoing order.

**Request Body:**

*   (OutgoingOrder object): The outgoing order to create.

**Responses:**

*   `201 Created`: Returns the created outgoing order.

### `PUT /api/OutgoingOrders/{id}`

Updates an outgoing order.

**Parameters:**

*   `id` (integer, required): The ID of the outgoing order to update.

**Request Body:**

*   (OutgoingOrder object): The updated outgoing order.

**Responses:**

*   `204 No Content`: If the outgoing order is updated successfully.
*   `400 Bad Request`: If the ID in the URL does not match the ID in the body.

### `DELETE /api/OutgoingOrders/{id}`

Deletes an outgoing order.

**Parameters:**

*   `id` (integer, required): The ID of the outgoing order to delete.

**Responses:**

*   `204 No Content`: If the outgoing order is deleted successfully.

### `POST /api/OutgoingOrders/{orderId}/items`

Adds an item to an outgoing order.

**Parameters:**

*   `orderId` (integer, required): The ID of the outgoing order.

**Request Body:**

*   (OutgoingOrderItem object): The item to add to the order.

**Responses:**

*   `204 No Content`: If the item is added successfully.
*   `400 Bad Request`: If the `orderId` in the URL does not match the `OutgoingOrderId` in the body.

### `GET /api/OutgoingOrders/{orderId}/items`

Gets all items for an outgoing order.

**Parameters:**

*   `orderId` (integer, required): The ID of the outgoing order.

**Responses:**

*   `200 OK`: Returns a list of items for the order.

---

## Photos

### `GET /api/Photos`

Gets a list of all photo filenames in the `wwwroot/uploads` directory.

**Responses:**

*   `200 OK`: Returns a list of photo filenames.

---

## Mystery

### `GET /api/Mystery/egg`

Returns a mystery message.

**Responses:**

*   `200 OK`: Returns a JSON object with a `message` property.
