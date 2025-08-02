using AspNetQueue.Services.JobQueue;
using AspNetQueue.Services.Jobs;
using AspNetQueue.Services.Jobs.Parameters;
using Microsoft.AspNetCore.Mvc;


namespace AspNetQueue.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobController(IJobQueue jobQueue) : ControllerBase
{
    [HttpPut("failingJob")]
    public IActionResult SuccessfulJob([FromQuery]string someParameter, [FromQuery]int count)
    {
        var parameters = SuccessfulJobParameters.New(someParameter, count);
        var status = jobQueue.QueueJob<SuccessfulJob, SuccessfulJobParameters>(parameters);
        return Ok(status.Message);
    }
    [HttpPut("successfulJob")]
    public IActionResult FailingJob()
    {
        var status = jobQueue.QueueJob<FailingJob>();
        return Ok(status.Message);
    }
    [HttpPut("longRunningJob")]
    public IActionResult LongRunningJob()
    {
        var status = jobQueue.QueueJob<LongRunningJob>();
        return Ok(status.Message);
    }

    [HttpGet("status")]
    public IActionResult GetJobStatuses()
    {
        var jobStatuses = jobQueue.GetAllJobStatuses();
        return Ok(jobStatuses);
    }
}
