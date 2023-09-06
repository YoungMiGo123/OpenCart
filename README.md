# OpenCart API

OpenCart is a RESTful API created for the management of shopping cart items in an e-commerce platform. It empowers users to execute Create, Read, Update, and Delete (CRUD) actions on their shopping cart items, attach images to items, and ensure secure authentication through GitHub OAuth 2.0.

## Authentication

OpenCart uses OAuth 2.0 for user authentication. Users are required to log in using their Github accounts.

### Github OAuth 2.0

To authenticate with Github:
1. Visit the login endpoint, you need to open this in a new tab.
2. You will be redirected to Github's authentication page, follow the relevant instructions.
3. After successfully logging in, you will be redirected back to the application and land on the swagger page.

## Endpoints

## Authentication

Before making requests to the OpenCart API, users must authenticate and obtain an access token. Authentication is not covered in this documentation.

## Cart Items

### Get All Cart Items

- **Endpoint:** `GET /api/OpenCart/GetCartItems`
- **Description:** Get a list of all cart items for the authenticated user.
- **Response:** List of cart items with associated metadata.

### Get Cart Item by ID

- **Endpoint:** `GET /api/OpenCart/GetCartItem/{cartItemId}`
- **Description:** Get details of a specific cart item by providing its ID.
- **Request Parameters:** `cartItemId` (Guid) - The ID of the cart item to retrieve.
- **Response:** Details of the requested cart item.

### Add Cart Item

- **Endpoint:** `POST /api/OpenCart/AddCartItem`
- **Description:** Add a new item to the shopping cart.
- **Request Body:** New cart item details.
  - `Name` (string, required) - The name of the item.
  - `Description` (string) - A description of the item.
  - `Price` (decimal, required) - The price of the item.
  - `Quantity` (int, required) - The quantity of the item.
- **Response:** Newly created cart item details.

### Update Cart Item

- **Endpoint:** `PUT /api/OpenCart/UpdateCartItem`
- **Description:** Update an existing cart item in the shopping cart.
- **Request Body:** Updated cart item details.
  - `Id` (Guid, required) - The ID of the cart item to update.
  - `Name` (string, required) - The updated name of the item.
  - `Description` (string) - The updated description of the item.
  - `Price` (decimal, required) - The updated price of the item.
  - `Quantity` (int, required) - The updated quantity of the item.
- **Response:** Updated cart item details.

### Remove Cart Item

- **Endpoint:** `DELETE /api/OpenCart/RemoveCartItem/{cartItemId}`
- **Description:** Remove a cart item from the shopping cart.
- **Request Parameters:** `cartItemId` (Guid) - The ID of the cart item to remove.
- **Response:** Success message indicating the removal of the cart item.

### Add Image to Cart Item

- **Endpoint:** `POST /api/OpenCart/AddImageToCartItem/{cartItemId}`
- **Description:** Add an image to an existing cart item.
- **Request Parameters:** `cartItemId` (Guid) - The ID of the cart item to which the image should be added.
- **Request Body:** Cart item image details.
  - `FileName` (string) - The name of the image file.
  - `FileBytes` (byte[]) - The byte array of the image file.
  - `ContentType` (string) - The content type of the image.
  - `Description` (string) - A description of the image.
  - `Name` (string) - The name of the image.
- **Response:** Details of the updated cart item with the newly added image.


## Documentation

API documentation is available using Swagger. You can access it by running the application and visiting `/swagger` in your browser.

## Model Validation

Input data is validated against predefined rules to ensure data integrity and consistency.

## Unit Testing

The application includes comprehensive unit tests to ensure the correctness of each endpoint and business logic.

## Relational Database

OpenCart utilizes a relational database (MSSQL) to store user data, cart items, and images.

## Containerization

The application can be containerized using Docker. To build and run the Docker container:
1. Build the image: 
2. Run the container: 

## HTTP Status Codes

The API responds with appropriate HTTP status codes for different scenarios to provide clear feedback to clients.

## Contributing

Contributions to OpenCart are welcome! Please open an issue or pull request for any enhancements or bug fixes.

## License

OpenCart is open-source and licensed under the [MIT License](LICENSE).
