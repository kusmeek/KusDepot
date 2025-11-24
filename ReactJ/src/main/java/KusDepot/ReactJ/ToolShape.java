package KusDepot.ReactJ;

import lombok.Data;
import java.util.UUID;
import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;

@Data
@JsonInclude(JsonInclude.Include.NON_NULL)
public class ToolShape
{
    @JsonProperty("id")
    private UUID id;

    @JsonProperty("x")
    private Double x;

    @JsonProperty("y")
    private Double y;

    @JsonProperty("circle")
    private Boolean circle;

    @JsonProperty("opacity")
    private Double opacity;

    @JsonProperty("rgb")
    private String rgb;

    @JsonProperty("rotation")
    private Double rotation;

    @JsonProperty("scale")
    private Double scale;

    @JsonProperty("sides")
    private Integer sides;

    @JsonProperty("star")
    private Boolean star;
}