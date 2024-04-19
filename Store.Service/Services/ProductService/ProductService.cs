using AutoMapper;
using Store.Data.Entities;
using Store.Repository.Interfaces;
using Store.Repository.Specification.Product;
using Store.Service.Helper;
using Store.Service.Services.ProductService.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<IReadOnlyList<BrandTypeDetailsDto>> GetAllBrandsAsync()
        {
            var brands = await unitOfWork.Repository<ProductBrand, int>().GetAllAsync();

            var mappedBrands = mapper.Map<IReadOnlyList<BrandTypeDetailsDto>>(brands);

            return mappedBrands;
        }
        public async Task<PaginatedResultDto<ProductDetailsDto>> GetAllProductsAsync(ProductSpecification input)
        { 
            var specs = new ProductsWithSpecifications(input);

            var products = await unitOfWork.Repository<Product, int>().GetAllWithSpecificationAsync(specs);

            var countSpecs = new ProductsWithFilterAndCountSpecifications(input);

            var count = await unitOfWork.Repository<Product, int>().CountSpecificationAsync(countSpecs);

            var mappedProducts = mapper.Map<IReadOnlyList<ProductDetailsDto>>(products);

            return new PaginatedResultDto<ProductDetailsDto>(input.PageIndex, input.PageSize, count, mappedProducts);
        }

        public async Task<IReadOnlyList<BrandTypeDetailsDto>> GetAllTypesAsync()
        {
            var types = await unitOfWork.Repository<ProductType, int>().GetAllAsync();

            var mappedTypes = mapper.Map<IReadOnlyList<BrandTypeDetailsDto>>(types);

            return mappedTypes;
        }

        public async Task<ProductDetailsDto> GetProductByIdAsync(int? id)
        {

            if (id is null)
                throw new Exception("Id is Null");

            var specs = new ProductsWithSpecifications(id);

            var product = await unitOfWork.Repository<Product, int>().GetWithSpecificationByIdAsync(specs);

            if (product is null)
                throw new Exception("Product Not Found");

            var mappedProduct = mapper.Map<ProductDetailsDto>(product);

            return mappedProduct;
        }
    }
}
