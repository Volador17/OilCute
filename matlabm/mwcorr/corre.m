function [r]=corre(x,y)
% 相关系数法，计算相关系数
% 输入：x，库光谱，y，参比光谱，
% 输出：r,相关系数
	r=(sum(x.*y))^2/(sum(x.*x)*sum(y.*y));
	
end
