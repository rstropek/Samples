import "bootstrap/dist/css/bootstrap.min.css";
import "./index.css";
import $ from "cash-dom";
import { ApplicationInsights } from '@microsoft/applicationinsights-web'

interface CalculateDto {
  valueIncl: number;
  valueExcl: number;
  vatPercentage: number;
}

$(() => {
  const appInsights = new ApplicationInsights({ config: {
    connectionString: '__APPINSIGHTS_CONNECTION_STRING__',
  } });
  appInsights.loadAppInsights();
  appInsights.trackPageView();

  $("#calculate").on("click", async () => {
    let valueIncl = parseInt($("#valueIncl").val() as string);
    let valueExcl = parseInt($("#valueExcl").val() as string);
    let vatPercentage = parseInt($("#vatPercentage").val() as string);

    if (isNaN(valueIncl) === isNaN(valueExcl)) {
      $("#error").text("Please enter either value incl. or value excl. VAT");
      $("#error").show();
      return;
    }
    if (isNaN(vatPercentage)) {
      $("#error").text("Please enter a VAT percentage");
      $("#error").show();
      return;
    }
    if (vatPercentage < 0 || vatPercentage > 100) {
      $("#error").text("Please enter a valid value for VAT percentage");
      $("#error").show();
      return;
    }

    $("#error").hide();

    const body: CalculateDto = {
      valueIncl,
      valueExcl,
      vatPercentage: vatPercentage / 100,
    };
    try {
      let response = await fetch(`__SERVICE_URL__/calculate`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(body),
      });
      let result: CalculateDto = await response.json();

      $("#valueIncl").val(result.valueIncl.toString());
      $("#valueExcl").val(result.valueExcl.toString());
    } catch (error) {
      $("#error").text(error.message);
      $("#error").show();
    }
  });
});
