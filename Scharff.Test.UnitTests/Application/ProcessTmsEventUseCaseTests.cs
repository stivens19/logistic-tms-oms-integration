using Moq;
using Scharff.Test.Application.DTOs;
using Scharff.Test.Application.Interfaces;
using Scharff.Test.Application.UseCases;
using Scharff.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scharff.Test.UnitTests.Application
{
    public class ProcessTmsEventUseCaseTests
    {
        private readonly Mock<IOrderRepository> _repoMock;
        private readonly Mock<ICloudStorageService> _storageMock;
        private readonly Mock<IClientNotifier> _notifierMock;
        private readonly ProcessTmsEventUseCase _useCase;

        public ProcessTmsEventUseCaseTests()
        {
            _repoMock = new Mock<IOrderRepository>();
            _storageMock = new Mock<ICloudStorageService>();
            _notifierMock = new Mock<IClientNotifier>();

            _notifierMock.Setup(n => n.AppliesTo(It.IsAny<string>())).Returns(true);

            _useCase = new ProcessTmsEventUseCase(
                _repoMock.Object,
                _storageMock.Object,
                new[] { _notifierMock.Object }
            );
        }

        [Fact]
        public async Task ExecuteAsync_WhenStatusAllowsEvidence_ShouldCallStorageService()
        {
            var eventDto = new TmsEventDto(
                "PICKUP",
                "STORE_WITHDRAWAL",
                "COLLECTED",
                "OK",
                "V1",
                "Courier",
                new TmsDetailsDto("ORD-55", "TRK-55", "CLI-01", "Cliente", null, null, new List<TmsEvidenceDto>
                {
                new("Foto", ".jpg", "foto.jpg", "http://test.com/foto.jpg")
                }),
                DateTime.UtcNow
            );

            _repoMock.Setup(r => r.GetByOrderNumberAsync("ORD-55")).ReturnsAsync((Order?)null);
            _storageMock.Setup(s => s.UploadAsync(It.IsAny<string>(), It.IsAny<TmsEvidenceDto>()))
                        .ReturnsAsync("https://storage/foto.jpg");

            await _useCase.ExecuteAsync(eventDto);

            _storageMock.Verify(s => s.UploadAsync("ORD-55", It.IsAny<TmsEvidenceDto>()), Times.Once);
            _repoMock.Verify(r => r.AddHistoryAsync(It.Is<OrderHistory>(h => h.OrderNumber == "ORD-55")), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenStatusDoesNotAllowEvidence_ShouldNotCallStorageService()
        {
            var eventDto = new TmsEventDto(
                "PICKUP",
                "STORE_WITHDRAWAL",
                "PLANNING",
                "OK",
                "V1",
                "Courier",
                new TmsDetailsDto("ORD-55", "TRK-55", "CLI-01", "Cliente", null, null, new List<TmsEvidenceDto>
                {
                new("Foto", ".jpg", "foto.jpg", "http://test.com/foto.jpg")
                }),
                DateTime.UtcNow
            );

            _repoMock.Setup(r => r.GetByOrderNumberAsync("ORD-55")).ReturnsAsync((Order?)null);

            await _useCase.ExecuteAsync(eventDto);

            _storageMock.Verify(s => s.UploadAsync(It.IsAny<string>(), It.IsAny<TmsEvidenceDto>()), Times.Never);
        }
    }
}
